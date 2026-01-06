namespace Application.DTOs.Auth;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public List<string> Details { get; set; } = new();

    public ErrorResponse() { }

    public ErrorResponse(string error, List<string>? details = null)
    {
        Error = error;
        Details = details ?? new List<string>();
    }
}
