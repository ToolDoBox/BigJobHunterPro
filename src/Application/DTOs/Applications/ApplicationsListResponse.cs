namespace Application.DTOs.Applications;

public class ApplicationsListResponse
{
    public List<ApplicationListDto> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
}
