using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fcg.Api.Middlewares
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMiddleware> _logger;

        public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                _logger.LogInformation("Request {method} {url} iniciada em {timestamp}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    DateTime.UtcNow);

                await _next(httpContext);

                _logger.LogInformation("Response {statusCode} para {method} {url} em {timestamp}",
                    httpContext.Response.StatusCode,
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado em {method} {url} em {timestamp}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    DateTime.UtcNow);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync("{\"error\": \"Ocorreu um erro interno no servidor.\"}");
            }
        }
    }

    public static class LogMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}