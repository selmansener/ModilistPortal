﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ModilistPortal.Data.DataAccess;
using ModilistPortal.Data.Repositories.AccountDomain;
using ModilistPortal.Data.Repositories.ProductDomain;
using ModilistPortal.Data.Repositories.SalesOrderDomain;
using ModilistPortal.Data.Repositories.TenantDomain;
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
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IProductExcelUploadRepository, ProductExcelUploadRepository>();
            services.AddScoped<IProductExcelRowRepository, ProductExcelRowRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

            return services;
        }

        public static IServiceCollection AddTransactionManager(this IServiceCollection services)
        {
            services.AddScoped<ITransactionManager, TransactionManager>();
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
