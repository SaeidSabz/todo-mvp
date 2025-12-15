namespace TodoMvp.Api.Contracts.Errors
{
    /// <summary>
    /// Represents a standardized API error response.
    /// </summary>
    public sealed class ApiErrorResponse
    {
        /// <summary>
        /// Gets or sets a short error code identifier.
        /// </summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the human-readable message for the error.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets a list of validation details or error messages.
        /// </summary>
        public IReadOnlyList<string>? Details { get; set; }
    }
}
