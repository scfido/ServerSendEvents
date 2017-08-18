using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SSE
{
    public class ServerEventsMiddleware
    {
        // Fields
        private readonly RequestDelegate _next;
        private readonly string path;

        // Constructors
        public ServerEventsMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        // Methods
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/sse")) //处理http://host/sse的请求
            {
                var response = context.Response;
                response.Headers.Add("Content-Type", "text/event-stream");

                for (var i = 0; true; ++i)
                {
                    // WriteAsync requires `using Microsoft.AspNetCore.Http`
                    await response
                        .WriteAsync($"data: 💓 Middleware {i} at {DateTime.Now}\r\r");

                    response.Body.Flush();
                    await Task.Delay(5 * 1000);
                }
            }

            // Next
            await _next.Invoke(context);
        }
    }

    public static partial class ServerEventsMiddlewareExtensions
    {
        public static IApplicationBuilder UseServerEvents(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ServerEventsMiddleware>();
        }
    }
}