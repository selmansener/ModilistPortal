using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using ModilistPortal.Business.PipelineBehaviors;

namespace ModilistPortal.Business.Extensions
{
    public static class BusinessExtensions
    {
        public static IServiceCollection AddCQRS(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(assembly);

            return services;
        }

        public static IMvcBuilder AddValidations(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<CreateAccountValidator>(includeInternalTypes: true);
                fv.AutomaticValidationEnabled = false;
                fv.ImplicitlyValidateChildProperties = true;
            });

            return mvcBuilder;
        }

        public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

        public static IServiceCollection AddLoggingBehavior(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            return services;
        }

        public static IServiceCollection AddTransactionBehavior(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            return services;
        }
    }
}
