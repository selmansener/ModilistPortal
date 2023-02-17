using Microsoft.ApplicationInsights.DataContracts;

using ModilistPortal.Infrastructure.Shared.Exntensions;
using ModilistPortal.Infrastructure.Shared.Utils;

namespace ModilistPortal.API.Middlewares
{
    public class TelemetryMiddleware
    {
        private readonly RequestDelegate _next;

        public TelemetryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Response.Body stream has to get swapped out with a stream that is buffered and supports seeking.
            // https://stackoverflow.com/questions/43403941/how-to-read-asp-net-core-response-body
            var originalBody = context.Response.Body;

            //Create a new memory stream...
            using (var responseBodyStream = new MemoryStream())
            {
                // Swap original Response.Body
                context.Response.Body = responseBodyStream;

                await LogRequestDetailsAsync(context);
                await _next(context);
                await LogResponseDetailsAsync(context);

                //Copy the contents of the new memory stream to the original stream, which is then returned to the client.
                await responseBodyStream.CopyToAsync(originalBody);
            }
        }

        private async Task LogRequestDetailsAsync(HttpContext httpContext)
        {
            try
            {
                var requestTelemetry = httpContext.Features.Get<RequestTelemetry>();
                var request = httpContext?.Request;

                if (request == null || requestTelemetry == null)
                {
                    return;
                }

                // Log IP Address
                if (!requestTelemetry.Context.GlobalProperties.ContainsKey(CustomProperties.ClientIpAddress))
                {
                    var clientIpAddress = httpContext.Connection?.RemoteIpAddress;
                    if (clientIpAddress != null)
                    {
                        requestTelemetry.Context.GlobalProperties.Add(CustomProperties.ClientIpAddress, clientIpAddress.ToString());
                    }
                }

                // Log Access Token
                if (!requestTelemetry.Context.GlobalProperties.ContainsKey(CustomProperties.AccessToken))
                {
                    string token = request.Headers.ContainsKey("Authorization")
                        ? request.Headers["Authorization"].ToString()
                        : null;

                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        requestTelemetry.Context.GlobalProperties.Add(CustomProperties.AccessToken, token);
                    }
                }

                // Log Request Body
                if (!requestTelemetry.Context.GlobalProperties.ContainsKey(CustomProperties.RequestBody))
                {
                    if (HttpMethods.IsPost(request.Method) || HttpMethods.IsPut(request.Method))
                    {
                        string requestBody = await request.ReadBodyAsync();
                        requestTelemetry.Context.GlobalProperties.Add(CustomProperties.RequestBody, requestBody);
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private async Task LogResponseDetailsAsync(HttpContext httpContext)
        {
            try
            {
                var requestTelemetry = httpContext.Features.Get<RequestTelemetry>();
                var response = httpContext?.Response;

                if (response == null || requestTelemetry == null)
                {
                    return;
                }

                // Log Response Status Code
                if (!requestTelemetry.Context.GlobalProperties.ContainsKey(CustomProperties.ResponseStatusCode))
                {
                    string responseStatusCode = response.StatusCode.ToString();
                    if (responseStatusCode != null)
                    {
                        requestTelemetry.Context.GlobalProperties.Add(CustomProperties.ResponseStatusCode, responseStatusCode);
                    }
                }

                // Log Response Body
                if (!requestTelemetry.Context.GlobalProperties.ContainsKey(CustomProperties.ResponseBody))
                {
                    string responseBody = await response.ReadBodyAsync();
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        requestTelemetry.Context.GlobalProperties.Add(CustomProperties.ResponseBody, responseBody);
                    }
                }

                // Log Tenant Id
                //if (!requestTelemetry.Context.GlobalProperties.ContainsKey(CustomProperties.TenantId))
                //{
                //    var tenantId = httpContext.User?.GetTenantIdAsGuid();
                //    if (tenantId.HasValue && tenantId.Value != default)
                //    {
                //        requestTelemetry.Context.GlobalProperties.Add(CustomProperties.TenantId, tenantId.ToString());
                //    }
                //}

                // Log User Id
                if (!requestTelemetry.Context.GlobalProperties.ContainsKey(CustomProperties.UserId))
                {
                    var userId = httpContext.User?.GetUserId();
                    if (userId.HasValue && userId.Value != default)
                    {
                        requestTelemetry.Context.GlobalProperties.Add(CustomProperties.UserId, userId.ToString());
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private static class CustomProperties
        {
            public const string TenantId = "tenantId";
            public const string UserId = "userId";
            public const string ClientIpAddress = "clienIpAddress";
            public const string AccessToken = "accesstoken";
            public const string RequestBody = "requestBody";
            public const string ResponseStatusCode = "responseStatusCode";
            public const string ResponseBody = "responseBody";
        }
    }
}
