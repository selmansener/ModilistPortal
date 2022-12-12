using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using ModilistPortal.API.Configuration;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModilistPortal.API.SwaggerConfiguration.Filters
{
    public class ApiKeyHeaderParameterOperationFilter : IOperationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiKeyHeaderParameterOperationFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var hasApiKey = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is ApiKeyAttribute);

            if (hasApiKey)
            {
                var environment = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();
                var devApiKey = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IOptions<ConfigurationOptions>>()?.Value?.DevelopmentApiKey;

                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-ApiKey",
                    In = ParameterLocation.Header,
                    Description = "X-ApiKey",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Default = new OpenApiString(environment.IsProduction() ? string.Empty : devApiKey)
                    }
                });
            }
        }
    }
}
