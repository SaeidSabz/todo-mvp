using Microsoft.AspNetCore.Diagnostics;
using TodoMvp.Api.Contracts.Errors;

namespace TodoMvp.Api.Infrastructure.Errors
{
    /// <summary>
    /// Handles unhandled exceptions and converts them into a standardized JSON response.
    /// </summary>
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger used to record unhandled exceptions.</param>
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Attempts to handle an exception by logging it and returning a safe error response.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// True if the exception was handled and a response was written; otherwise false.
        /// </returns>
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception. TraceId={TraceId}, Method={Method}, Path={Path}",
                traceId,
                httpContext.Request.Method,
                httpContext.Request.Path.Value);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var body = new ApiErrorResponse
            {
                Error = "ServerError",
                Message = "An unexpected error occurred.",
                Details = new[] { $"TraceId: {traceId}" }
            };

            await httpContext.Response.WriteAsJsonAsync(body, cancellationToken);

            // Returning true signals: "we handled it"
            return true;
        }
    }
}
