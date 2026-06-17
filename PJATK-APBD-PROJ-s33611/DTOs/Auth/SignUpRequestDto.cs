using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_PROJ_s33611.DTOs.Auth;

public record SignUpRequestDto
{
    [Required] 
    public string Login { get; set; } = string.Empty;
    
    [Required] 
    public string Password { get; set; } = string.Empty;
    
    [Required, Compare(nameof(Password))] 
    public string RepeatPassword { get; set; } = string.Empty;
}