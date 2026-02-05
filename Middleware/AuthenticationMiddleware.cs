namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // ALWAYS allow Swagger through
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // For API calls, check authentication
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                // Skip auth for GET /api/users (public)
                if (context.Request.Path == "/api/users" && context.Request.Method == "GET")
                {
                    await _next(context);
                    return;
                }

                // Check for Authorization header
                if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Missing Authorization header");
                    return;
                }

                var token = authHeader.ToString().Replace("Bearer ", "");
                if (token != "valid-token-123")
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
            }

            await _next(context);
        }
    }
}