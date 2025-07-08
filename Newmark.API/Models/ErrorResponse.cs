using System.Text.Json.Serialization;

namespace Newmark.API.Models
{
    /// <summary>
    /// Represents a standardized error response for API operations
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// A short, human-readable summary of the error
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The HTTP status code associated with this error
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        /// A detailed description of the error
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;

        /// <summary>
        /// A unique identifier for this error occurrence
        /// </summary>
        [JsonPropertyName("traceId")]
        public string? TraceId { get; set; }

        /// <summary>
        /// The timestamp when the error occurred
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Creates a new ErrorResponse with default values
        /// </summary>
        public ErrorResponse() { }

        /// <summary>
        /// Creates a new ErrorResponse with specified values
        /// </summary>
        /// <param name="title">The error title</param>
        /// <param name="status">The HTTP status code</param>
        /// <param name="detail">The error detail</param>
        /// <param name="traceId">Optional trace ID</param>
        public ErrorResponse(string title, int status, string detail, string? traceId = null)
        {
            Title = title;
            Status = status;
            Detail = detail;
            TraceId = traceId;
        }

        /// <summary>
        /// Creates a 500 Internal Server Error response
        /// </summary>
        /// <param name="detail">The error detail</param>
        /// <param name="traceId">Optional trace ID</param>
        /// <returns>An ErrorResponse with status 500</returns>
        public static ErrorResponse InternalServerError(string detail, string? traceId = null)
        {
            return new ErrorResponse("Internal Server Error", 500, detail, traceId);
        }

        /// <summary>
        /// Creates a 404 Not Found Error response
        /// </summary>
        /// <param name="detail">The error detail</param>
        /// <param name="traceId">Optional trace ID</param>
        /// <returns>An ErrorResponse with status 404</returns>
        public static ErrorResponse NotFound(string detail, string? traceId = null)
        {
            return new ErrorResponse("Not Found", 404, detail, traceId);
        }

        /// <summary>
        /// Creates a 400 Bad Request Error response
        /// </summary>
        /// <param name="detail">The error detail</param>
        /// <param name="traceId">Optional trace ID</param>
        /// <returns>An ErrorResponse with status 400</returns>
        public static ErrorResponse BadRequest(string detail, string? traceId = null)
        {
            return new ErrorResponse("Bad Request", 400, detail, traceId);
        }
    }
}
