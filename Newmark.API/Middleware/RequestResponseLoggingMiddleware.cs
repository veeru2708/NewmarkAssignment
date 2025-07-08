using System.Diagnostics;
using System.Text;

namespace Newmark.API.Middleware
{
    /// <summary>
    /// Middleware for logging HTTP requests and responses
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the RequestResponseLoggingMiddleware
        /// </summary>
        /// <param name="next">The next middleware in the pipeline</param>
        /// <param name="logger">Logger instance</param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invokes the middleware to log request and response information
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            // Log request
            await LogRequestAsync(context, requestId);

            // Capture the original response body stream
            var originalResponseBodyStream = context.Response.Body;

            try
            {
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Call the next middleware
                await _next(context);

                // Log response
                await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds, responseBodyStream);

                // Copy the response back to the original stream
                responseBodyStream.Position = 0;
                await responseBodyStream.CopyToAsync(originalResponseBodyStream);
            }
            finally
            {
                context.Response.Body = originalResponseBodyStream;
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// Logs HTTP request information
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <param name="requestId">Unique request identifier</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task LogRequestAsync(HttpContext context, string requestId)
        {
            try
            {
                var request = context.Request;
                var requestBody = string.Empty;

                // Read request body for POST/PUT requests
                if (request.Method == "POST" || request.Method == "PUT")
                {
                    request.EnableBuffering();
                    using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }

                _logger.LogInformation(
                    "Request {RequestId}: {Method} {Path} {QueryString} - Body: {Body}",
                    requestId,
                    request.Method,
                    request.Path,
                    request.QueryString,
                    string.IsNullOrEmpty(requestBody) ? "N/A" : requestBody
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging request {RequestId}", requestId);
            }
        }

        /// <summary>
        /// Logs HTTP response information
        /// </summary>
        /// <param name="context">The HTTP context</param>
        /// <param name="requestId">Unique request identifier</param>
        /// <param name="elapsedMilliseconds">Time taken to process the request</param>
        /// <param name="responseBodyStream">Response body stream</param>
        /// <returns>A task representing the asynchronous operation</returns>
        private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMilliseconds, MemoryStream responseBodyStream)
        {
            try
            {
                var response = context.Response;
                responseBodyStream.Position = 0;
                
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

                _logger.LogInformation(
                    "Response {RequestId}: {StatusCode} - {ElapsedMs}ms - Body: {Body}",
                    requestId,
                    response.StatusCode,
                    elapsedMilliseconds,
                    string.IsNullOrEmpty(responseBody) ? "N/A" : responseBody
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging response {RequestId}", requestId);
            }
        }
    }

    /// <summary>
    /// Extension methods for adding the RequestResponseLoggingMiddleware
    /// </summary>
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the RequestResponseLoggingMiddleware to the application pipeline
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
