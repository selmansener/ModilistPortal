using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ModilistPortal.Infrastructure.Shared.Utils
{
    public static class HttpContextUtils
    {
        public static async Task<string> FormatRequestAsync(this HttpRequest request)
        {
            return $"Request: {request.Scheme} {request.Host}{request.Path} {request.QueryString} {await request.ReadBodyAsync()}";
        }

        public static async Task<string> ReadBodyAsync(this HttpRequest request)
        {
            request.EnableBuffering();

            request.Body.Position = 0;
            byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            string requestBody = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;

            return requestBody;
        }

        public static async Task<string> FormatResponseAsync(this HttpResponse response)
        {
            return $"Response: {response.StatusCode}: {await response.ReadBodyAsync()}";
        }

        public static async Task<string> ReadBodyAsync(this HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[Convert.ToInt32(response.Body.Length)];
            await response.Body.ReadAsync(buffer, 0, buffer.Length);
            string requestBody = Encoding.UTF8.GetString(buffer);
            response.Body.Seek(0, SeekOrigin.Begin);

            return requestBody;
        }
    }
}
