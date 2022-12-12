using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace ModilistPortal.API.SwaggerConfiguration.Filters
{
    public sealed class SwaggerNullPropertyAttribute : Attribute { }

    /// <summary>
    /// Makes all value-type properties "Required" in the schema docs, which is appropriate since they cannot be null.
    /// </summary>
    /// <remarks>
    /// This saves effort + maintenance from having to add <c>[Required]</c> to all value type properties; Web API, EF, and Json.net already understand
    /// that value type properties cannot be null.
    public sealed class RequireValueTypePropertiesSchemaFilter : ISchemaFilter
    {
        private readonly CamelCasePropertyNamesContractResolver _camelCaseContractResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireValueTypePropertiesSchemaFilter"/> class.
        /// </summary>
        /// <param name="camelCasePropertyNames">If <c>true</c>, property names are expected to be camel-cased in the JSON schema.</param>
        /// <remarks>
        /// I couldn't figure out a way to determine if the swagger generator is using <see cref="CamelCaseNamingStrategy"/> or not;
        /// so <paramref name="camelCasePropertyNames"/> needs to be passed in since it can't be determined.
        /// </remarks>
        public RequireValueTypePropertiesSchemaFilter(bool camelCasePropertyNames)
        {
            _camelCaseContractResolver = camelCasePropertyNames ? new CamelCasePropertyNamesContractResolver() : null;
        }

        /// <summary>
        /// Adds non-nullable value type properties in a <see cref="Type"/> to the set of required properties for that type.
        /// </summary>
        /// <param name="model">OpenApiSchema.</param>
        /// <param name="context">SchemaFilterContext.</param>
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            foreach (var property in context.Type.GetProperties())
            {
                string schemaPropertyName = PropertyName(property);
                // This check ensures that properties that are not in the schema are not added as required.
                // This includes properties marked with [IgnoreDataMember] or [JsonIgnore] (should not be present in schema or required).
                if (model.Properties?.ContainsKey(schemaPropertyName) == true)
                {
                    // Value type properties are required,
                    // except: Properties of type Nullable<T> are not required.
                    var propertyType = property.PropertyType;
                    if (!property.CustomAttributes.Any(attr => attr.AttributeType == typeof(SwaggerNullPropertyAttribute))
                        && (
                                (
                                    propertyType.IsValueType
                                    && !(propertyType.IsConstructedGenericType && (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                                )
                                || (propertyType == typeof(string))
                           )
                       )
                    {
                        // Properties marked with [Required] are already required (don't require it again).
                        if (!property.CustomAttributes.Any(attr => attr.AttributeType == typeof(RequiredAttribute)))
                        {
                            // Make the value type property required
                            if (model.Required == null)
                            {
                                model.Required = new HashSet<string>();
                            }

                            model.Required.Add(schemaPropertyName);
                        }
                    }

                    model.Properties[schemaPropertyName].Nullable = false;
                }
            }
        }

        /// <summary>
        /// Returns the JSON property name for <paramref name="property"/>.
        /// </summary>
        /// <param name="property">PropertyInfo.</param>
        /// <returns>string name of property.</returns>
        private string PropertyName(PropertyInfo property)
        {
            return _camelCaseContractResolver?.GetResolvedPropertyName(property.Name) ?? property.Name;
        }
    }
}
