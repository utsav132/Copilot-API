using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string AuthHeaderName = "Authorization";
        private const string ExpectedToken = "Bearer HypeTech-Secure-Token-2026";

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(AuthHeaderName, out var extractedToken))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Authentication token is missing.\"}");
                return;
            }

            if (!ExpectedToken.Equals(extractedToken))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Unauthorized access. Invalid token.\"}");
                return;
            }

            await _next(context);
        }
    }
}
