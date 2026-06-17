using System.ComponentModel.DataAnnotations;

namespace PJATK_APBD_PROJ_s33611.DTOs.Auth;

public record SignInRequestDto(
    [Required] 
    string Login,
    [Required] 
    string Password);