using System;
using System.IO;
using System.Text;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ModilistPortal.Business.Extensions;
using ModilistPortal.Data.Extensions;
using ModilistPortal.Data.Transactions;
using ModilistPortal.Functions.EventHandlers.Configurations;
using ModilistPortal.Infrastructure.Azure.Extensions;
using ModilistPortal.Infrastructure.Azure.Extensions.Configurations;
using ModilistPortal.Infrastructure.Shared.Configurations;

using Newtonsoft.Json;

[assembly: FunctionsStartup(typeof(ModilistPortal.Functions.EventHandlers.Startup))]
namespace ModilistPortal.Functions.EventHandlers
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var options = GetConfigurations(env);

            builder.Services.Configure<StorageConnectionStrings>(config =>
            {
                config.AppStorage = options.AppStorage;
            });

            builder.Services.Configure<EventGridClientOptions>(config =>
            {
                config.Secret = options.EventGridClientOptions.Secret;
                config.Url = options.EventGridClientOptions.Url;
            });

            builder.Services.AddBlobClientFactory();
            var environment = builder.Services.BuildServiceProvider().GetService<IHostEnvironment>();

            builder.Services.AddDataAccess(options.ModilistDbConnectionOptions, environment);

            builder.Services.AddRepositories();

            builder.Services.AddCQRS();

            builder.Services.AddLoggingBehavior();

            builder.Services.AddValidationBehavior();

            builder.Services.AddTransactionBehavior();

            builder.Services.AddTransactionManager();

            builder.Services.BuildServiceProvider();
        }

        public ConfigurationOptions GetConfigurations(string environmentName)
        {
            var assembly = typeof(Startup).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"ModilistPortal.Functions.EventHandlers.Settings.appsettings.{environmentName}.json"))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string options = reader.ReadToEnd();
                if (string.IsNullOrEmpty(options))
                {
                    throw new InvalidOperationException($"Could not load appsettings file.");
                }

                return JsonConvert.DeserializeObject<ConfigurationOptions>(options);
            }
        }
    }
}
