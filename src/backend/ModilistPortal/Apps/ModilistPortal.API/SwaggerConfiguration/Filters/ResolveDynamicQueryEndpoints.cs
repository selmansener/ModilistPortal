using System.Reflection;

using DynamicQueryBuilder;

using Microsoft.OpenApi.Models;

using ModilistPortal.API.SwaggerConfiguration.Attributes;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModilistPortal.API.SwaggerConfiguration.Filters
{
    public sealed class ResolveDynamicQueryEndpoints : IOperationFilter
    {
        private readonly string _description;
        private readonly string _dqbResolveParam;

        public ResolveDynamicQueryEndpoints(
            string dqbResolveParam = "",
            string description = "DynamicQuery")
        {
            _description = description;
            _dqbResolveParam = dqbResolveParam;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context
                .MethodInfo
                .GetCustomAttribute<DynamicQueryAttribute>() != null)
            {
                OpenApiSchema apiSchema;
                if (operation.Parameters != null)
                {
                    operation.Parameters.Clear();

                    apiSchema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository);
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        In = ParameterLocation.Query,
                        Schema = apiSchema,
                        Description = _description,
                        Name = _dqbResolveParam
                    });

                    var methodParams = context.MethodInfo.GetParameters();

                    foreach (var methodParam in methodParams)
                    {
                        if (methodParam.GetCustomAttribute<SwaggerIncludeAttribute>() != null)
                        {
                            apiSchema = context.SchemaGenerator.GenerateSchema(methodParam.ParameterType, context.SchemaRepository);
                            if (context.ApiDescription.ActionDescriptor.Parameters.Where(p => p.Name == methodParam.Name).Any(p => p.BindingInfo.BindingSource.DisplayName == "Path"))
                            {
                                operation.Parameters.Add(new OpenApiParameter
                                {
                                    In = ParameterLocation.Path,
                                    Name = methodParam.Name,
                                    Required = true,
                                    Schema = apiSchema
                                });
                            }
                            else
                            {
                                operation.Parameters.Add(new OpenApiParameter
                                {
                                    In = ParameterLocation.Query,
                                    Name = methodParam.Name,
                                    Required = true,
                                    Schema = apiSchema
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
