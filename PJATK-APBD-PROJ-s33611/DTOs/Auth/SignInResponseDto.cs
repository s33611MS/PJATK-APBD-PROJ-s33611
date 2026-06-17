namespace PJATK_APBD_PROJ_s33611.DTOs.Auth;

public record SignInResponseDto(
    string AccessToken,
    string RefreshToken
    );