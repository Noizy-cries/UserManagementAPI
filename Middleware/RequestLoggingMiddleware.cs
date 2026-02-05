namespace UserManagementAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            var request = context.Request;

            // Log request details
            _logger.LogInformation($"Incoming Request: {request.Method} {request.Path} {request.QueryString}");
            _logger.LogDebug($"Headers: {string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");

            // Enable buffering to read request body multiple times
            request.EnableBuffering();

            // Read request body if it's not too large
            string requestBody = await ReadRequestBody(request);
            if (!string.IsNullOrEmpty(requestBody))
            {
                _logger.LogDebug($"Request Body: {requestBody}");
            }

            // Reset request body stream position for next middleware
            request.Body.Position = 0;

            // Capture original response body stream
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                // Log response details
                _logger.LogInformation($"Response: {context.Response.StatusCode} - Duration: {duration.TotalMilliseconds:F2}ms");

                // Read response body
                responseBody.Seek(0, SeekOrigin.Begin);
                string responseBodyContent = await new StreamReader(responseBody).ReadToEndAsync();
                _logger.LogDebug($"Response Body: {responseBodyContent}");

                // Copy back to original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private static async Task<string> ReadRequestBody(HttpRequest request)
        {
            try
            {
                if (request.Body == null || !request.Body.CanRead)
                    return string.Empty;

                // Only read body for certain content types
                if (request.ContentType?.Contains("application/json") == true ||
                    request.ContentType?.Contains("application/xml") == true ||
                    request.ContentType?.Contains("text/") == true)
                {
                    using var reader = new StreamReader(request.Body, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    return body.Length > 1000 ? body.Substring(0, 1000) + "..." : body;
                }
            }
            catch (Exception)
            {
                // Don't fail the request if we can't read the body
            }

            return string.Empty;
        }
    }
}