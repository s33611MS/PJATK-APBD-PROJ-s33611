using PJATK_APBD_PROJ_s33611.DTOs.Auth;
using PJATK_APBD_PROJ_s33611.Entities.Auth;
using PJATK_APBD_PROJ_s33611.Exceptions;
using PJATK_APBD_PROJ_s33611.Repositories.Auth;

namespace PJATK_APBD_PROJ_s33611.Services.Auth;

public class AuthService(IAuthRepository repo, IPasswordService passwordService, ITokenService tokenService) : IAuthService
{
    public async Task<SignInResponseDto> SignInAsync(SignInRequestDto request, CancellationToken cancellationToken)
    {
        var user = await repo.GetUserByLoginAsync(request.Login, cancellationToken)
                   ?? throw new UnauthorizedException("Invalid login or password");

        if (!passwordService.VerifyHashedPassword(user.PasswordHash, request.Password))
            throw new UnauthorizedException("Invalid login or password");

        var accessToken = tokenService.GenerateAccessToken(
            user.Id.ToString(),
            user.Login,
            user.UserRole.Name
        );
        var refreshToken = tokenService.GenerateRefreshToken();

        var token = await repo.GetTokenByIdAsync(user.Id, cancellationToken);
        if (token is null)
        {
            token = new Token
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await repo.AddTokenAsync(token, cancellationToken);
        }
        else
        {
            token.RefreshToken = refreshToken;
            token.ExpiresAt = DateTime.UtcNow.AddHours(2);
            await repo.SaveChangesAsync(cancellationToken);
        }

        return new SignInResponseDto(accessToken, refreshToken);
    }

    public async Task<SignInResponseDto> SignUpAsync(SignUpRequestDto request, CancellationToken cancellationToken)
    {
        if (await repo.GetUserByLoginAsync(request.Login, cancellationToken) is not null)
            throw new ConflictException("This login is already taken");

        var userRole = await repo.GetUserRoleAsync(cancellationToken)
            ?? throw new NotFoundException("There is no 'User' role in the database");

        var user = new User
        {
            Login = request.Login,
            PasswordHash = passwordService.HashPassword(request.Password),
            UserRole = userRole
        };
        
        await repo.AddUserAsync(user, cancellationToken);

        var accessToken = tokenService.GenerateAccessToken(
            user.Id.ToString(),
            user.Login,
            userRole.Name
        );
        var refreshToken = tokenService.GenerateRefreshToken();

        var token = new Token
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(2)
        };
        
        await repo.AddTokenAsync(token, cancellationToken);

        return new SignInResponseDto(accessToken, refreshToken);
    }

    public async Task<SignInResponseDto> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var user = await repo.GetUserByRefreshTokenAsync(refreshToken, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token");
        
        var accessToken = tokenService.GenerateAccessToken(
            user.Id.ToString(),
            user.Login,
            user.UserRole.Name
        );
        var newRefreshToken = tokenService.GenerateRefreshToken();

        var token = await repo.GetTokenByIdAsync(user.Id, cancellationToken);
        if (token is null)
        {
            token ??= new Token
            {
                UserId = user.Id,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await repo.AddTokenAsync(token, cancellationToken);
        }
        else
        {
            token.RefreshToken = newRefreshToken;
            token.ExpiresAt = DateTime.UtcNow.AddHours(2);
            await repo.SaveChangesAsync(cancellationToken);
        }

        return new SignInResponseDto(accessToken, newRefreshToken);
    }

    public async Task SignOutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        await repo.DeleteTokenAsync(refreshToken, cancellationToken);
    }
}