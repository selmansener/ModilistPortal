using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ModilistPortal.API.Configuration;

namespace ModilistPortal.API.Filters
{
    public class ApiKeyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var devApiKey = context.HttpContext.RequestServices.GetRequiredService<IOptions<ConfigurationOptions>>()?.Value?.DevelopmentApiKey;

            if (string.IsNullOrEmpty(devApiKey) || !Guid.TryParse(devApiKey, out Guid devApiKeyGuid))
            {
                throw new InvalidOperationException("Cannot read devApiKey option");
            }

            if (!context.HttpContext.Request.Headers.TryGetValue("X-ApiKey", out var apiKey))
            {
                throw new UnauthorizedAccessException();
            }

            var key = apiKey.FirstOrDefault();

            if (string.IsNullOrEmpty(key) || !Guid.TryParse(key, out Guid apiKeyGuid))
            {
                throw new UnauthorizedAccessException();
            }

            if (devApiKeyGuid != apiKeyGuid)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
