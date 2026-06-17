namespace PJATK_APBD_PROJ_s33611.Services.Auth;

public interface ITokenService
{
    string GenerateAccessToken(string userId, string username, string userRole);
    string GenerateRefreshToken();
}