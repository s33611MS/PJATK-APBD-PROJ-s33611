using PJATK_APBD_PROJ_s33611.DTOs.Auth;

namespace PJATK_APBD_PROJ_s33611.Services.Auth;

public interface IAuthService
{
    public Task<SignInResponseDto> SignInAsync(SignInRequestDto request, CancellationToken cancellationToken);
    public Task<SignInResponseDto> SignUpAsync(SignUpRequestDto request, CancellationToken cancellationToken);
    public Task<SignInResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
    public Task SignOutAsync(string refreshToken, CancellationToken cancellationToken);
}