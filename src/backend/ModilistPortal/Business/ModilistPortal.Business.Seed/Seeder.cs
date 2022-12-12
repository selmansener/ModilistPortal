using System.Diagnostics;
using System.Text;

using EFCoreAutoMigrator;

using JsonNet.ContractResolvers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ModilistPortal.Business.Seed.Configuration;
using ModilistPortal.Business.Seed.Data;
using ModilistPortal.Business.Seed.Services.Base;
using ModilistPortal.Data.DataAccess;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ModilistPortal.Business.Seed
{
    public interface ISeeder
    {
        Task Seed(SeedServiceType service, CancellationToken cancellationToken);

        void ClearExecutedServices();
    }

    internal class Seeder : ISeeder
    {
        private readonly ILogger<Seeder> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly SeedCache _seedCache;
        private readonly SeedServices _seedServices;
        private readonly ModilistPortalDbContext _dbContext;
        private readonly IHostEnvironment _hostEnvironment;

        public Seeder(ILogger<Seeder> logger, IServiceProvider serviceProvider, SeedCache seedCache, SeedServices seedServices, ModilistPortalDbContext dbContext, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _seedCache = seedCache;
            _seedServices = seedServices;
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
        }

        public void ClearExecutedServices()
        {
            if (_hostEnvironment.IsProduction())
            {
                throw new NotSupportedException("Auto Migrations are not supported for production environment.");
            }

            var assembly = typeof(SeedData).Assembly;
            using (var stream = assembly.GetManifestResourceStream("ModilistPortal.Business.Seed.ClearDatabase.sql"))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string sql = reader.ReadToEnd();
                if (string.IsNullOrEmpty(sql))
                {
                    throw new InvalidOperationException($"Could not load ClearData file. (ModilistPortal.Business.Seed.ClearDatabase.sql)");
                }

                _dbContext.Database.ExecuteSqlRaw(sql);
            }

            var script = _dbContext.Database.GenerateCreateScript();

            var autoMigrator = new AutoMigrator(_dbContext, new BlacklistedSource[] { }, new AutoMigratorOptions
            {
                LoggingEnabled = true
            }, _logger);

            autoMigrator.Migrate(true, MigrationModelHashStorageMode.Database);
            _seedCache.Clear();
        }

        public async Task Seed(SeedServiceType service, CancellationToken cancellationToken)
        {
            if (_hostEnvironment.IsProduction())
            {
                throw new NotSupportedException("Auto Migrations are not supported for production environment.");
            }

            try
            {
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);

                await ResolveAndSeed(service, cancellationToken);

                _dbContext.ChangeTracker.DetectChanges();

                await _dbContext.SaveChangesAsync(cancellationToken);

                await _dbContext.Database.CommitTransactionAsync(cancellationToken);

                _seedCache.UpdateSeedCacheData();
            }
            catch (Exception ex)
            {
                await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private async Task ResolveAndSeed(SeedServiceType service, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initializing seed service {0}", service);

            var seedService = _seedServices.GetService(_serviceProvider, service);

            _logger.LogInformation("Resolving dependencies for {0}", service);

            var dependencies = _seedCache.FindUnexecutedServices(seedService.GetDependencies());

            _logger.LogInformation("{0} dependencies found for {1}", dependencies.Count(), service);

            foreach (var serviceDependency in dependencies)
            {
                await ResolveAndSeed(serviceDependency, cancellationToken);
            }

            _logger.LogInformation("Executing seed service {0}", service);

            await seedService.Execute(cancellationToken);

            _seedCache.AddExecutedService(service);

            _dbContext.ChangeTracker.DetectChanges();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public static class SeedServiceInitializer
    {
        public static void AddSeedServices(this IServiceCollection services, string environment)
        {
            Console.WriteLine(environment);
            Debug.Write(environment);

            SeedData seedData;
            var assembly = typeof(SeedData).Assembly;

            string resourceName = $"ModilistPortal.Business.Seed.Data.DataFiles.SeedData.{environment}.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string data = reader.ReadToEnd();
                if (string.IsNullOrEmpty(data))
                {
                    throw new InvalidOperationException($"Could not load seed data file. ({resourceName})");
                }

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                settings.Converters.Add(new StringEnumConverter());

                seedData = JsonConvert.DeserializeObject<SeedData>(data, settings);

                services.AddSingleton(seedData);
            }

            var seedServices = new SeedServices();
            var baseSeedServiceType = typeof(BaseSeedService);
            var seedServiceTypes = baseSeedServiceType.Assembly
                .GetTypes()
                .Where(x => !x.IsAbstract && x.BaseType == baseSeedServiceType);

            foreach (var serviceType in seedServiceTypes)
            {
                services.AddScoped(serviceType);
                seedServices.Add(serviceType.Name.Replace("SeedService", string.Empty), serviceType);
            }

            services.AddSingleton(seedServices);
            services.AddSingleton(new SeedCache());
            services.AddScoped<ISeeder, Seeder>();
        }
    }
}