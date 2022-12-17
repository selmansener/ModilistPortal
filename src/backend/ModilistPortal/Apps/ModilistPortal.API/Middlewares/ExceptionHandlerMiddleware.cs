using FluentValidation;

using ModilistPortal.API.Models;
using ModilistPortal.Infrastructure.Shared.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ModilistPortal.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly string _exceptionResponseContentType = "application/json";
        private readonly RequestDelegate _next;
        private ILogger<ExceptionHandlerMiddleware> _logger;
        private IWebHostEnvironment _environment;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger, IWebHostEnvironment environment)
        {
            try
            {
                _logger = logger;
                _environment = environment;

                await _next.Invoke(context);
            }
            catch (Exception exception) when (exception is IClientException clientException)
            {
                var exceptionResponse = new ResponseModel<object>(clientException.StatusCode, null, exception.Message, exception.GetType().Name, null);

                context.Response.StatusCode = clientException.StatusCode;
                await SendErrorResponse(context, exceptionResponse);
            }
            catch (Exception exception) when (exception is ValidationException validationException)
            {
                var exceptionResponse = new ResponseModel<object>(400, null, validationException.Message, validationException.GetType().Name, GetErrors(validationException));

                context.Response.StatusCode = 400;
                await SendErrorResponse(context, exceptionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                var exceptionResponse = new ResponseModel<object>(500, null, _environment.IsProduction() ? null : ex.Message, _environment.IsProduction() ? "Unexpected" : ex.GetType().Name, null);

                context.Response.StatusCode = 500;
                await SendErrorResponse(context, exceptionResponse);
            }
        }

        private async Task SendErrorResponse(HttpContext context, ResponseModel<object> exceptionResponse)
        {
            var exceptionResponseBody = JsonConvert.SerializeObject(exceptionResponse, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });

            context.Response.ContentType = _exceptionResponseContentType;
            await context.Response.WriteAsync(exceptionResponseBody);
        }

        private IDictionary<string, List<string>> GetErrors(ValidationException validationException)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            var groupedErrors = validationException.Errors.GroupBy(x => x.PropertyName);

            foreach (var groupedError in groupedErrors)
            {
                foreach (var error in groupedError)
                {

                    if (errors.ContainsKey(groupedError.Key))
                    {
                        errors[groupedError.Key].Add(error.ErrorCode);
                    }
                    else
                    {
                        errors.Add(groupedError.Key, new List<string>()
                            {
                                error.ErrorCode
                            });
                    }
                }
            }

            return errors;
        }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionHandlerMiddleware>();

            return builder;
        }
    }
}
