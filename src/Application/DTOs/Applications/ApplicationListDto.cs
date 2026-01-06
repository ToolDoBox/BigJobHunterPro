namespace Application.DTOs.Applications;

public class ApplicationListDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
