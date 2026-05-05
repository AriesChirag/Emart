namespace Emart_DotNet.DTOs
{
    /// <summary>
    /// Standard error response DTO for consistent error handling across the API
    /// </summary>
    public class ErrorResponseDTO
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Path { get; set; } = string.Empty;

        public ErrorResponseDTO() { }

        public ErrorResponseDTO(int statusCode, string message, string details = "", string path = "")
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
            Path = path;
            Timestamp = DateTime.UtcNow;
        }
    }
}
