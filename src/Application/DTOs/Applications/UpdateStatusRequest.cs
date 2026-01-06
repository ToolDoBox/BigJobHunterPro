using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Applications;

public class UpdateStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    public ApplicationStatus Status { get; set; }
}
