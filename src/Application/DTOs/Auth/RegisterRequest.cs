using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(256, ErrorMessage = "Email must not exceed 256 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(100, ErrorMessage = "Password must not exceed 100 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Display name is required")]
    [MinLength(2, ErrorMessage = "Display name must be at least 2 characters")]
    [MaxLength(100, ErrorMessage = "Display name must not exceed 100 characters")]
    public string DisplayName { get; set; } = string.Empty;
}
