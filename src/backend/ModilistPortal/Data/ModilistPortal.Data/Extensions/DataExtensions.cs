using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Data.Transactions;
using ModilistPortal.Infrastructure.Shared.Configurations;

namespace ModilistPortal.Data.Extensions
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, DbConnectionOptions dbConnectionOptions, IHostEnvironment environment)
        {
            string dbConnectionString = new SqlConnectionStringBuilder
            {
                DataSource = dbConnectionOptions.Server,
                UserID = dbConnectionOptions.UserName,
                Password = dbConnectionOptions.Password,
                InitialCatalog = dbConnectionOptions.Database
            }.ConnectionString;

            services.AddDbContext<ModilistPortalDbContext>(opts =>
            {
                if (environment.IsDevelopment())
                {
                    opts.EnableSensitiveDataLogging();
                }

                opts.UseSqlServer(dbConnectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(120);
                });
            });

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {

            return services;
        }

        public static IServiceCollection AddTransactionManager(this IServiceCollection services, RegistrationType registrationType = RegistrationType.Scoped)
        {
            switch (registrationType)
            {
                case RegistrationType.Singleton:
                    services.AddSingleton<ITransactionManager, TransactionManager>();
                    break;
                case RegistrationType.Scoped:
                    services.AddScoped<ITransactionManager, TransactionManager>();
                    break;
                case RegistrationType.Transient:
                    services.AddTransient<ITransactionManager, TransactionManager>(provider =>
                    {
                        return new TransactionManager(provider.GetRequiredService<ModilistPortalDbContext>(), shouldDisposeDbContext: false);
                    });
                    break;
                default:
                    throw new InvalidOperationException($"Unknown registration type sent: {registrationType}");
            }

            return services;
        }
    }

    public enum RegistrationType
    {
        Singleton,
        Scoped,
        Transient
    }
}
