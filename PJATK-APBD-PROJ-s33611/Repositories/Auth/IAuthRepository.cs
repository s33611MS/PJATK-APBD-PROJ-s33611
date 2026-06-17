using PJATK_APBD_PROJ_s33611.Entities.Auth;

namespace PJATK_APBD_PROJ_s33611.Repositories.Auth;

public interface IAuthRepository
{
    Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<Token?> GetTokenByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserRole?> GetUserRoleAsync(CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task AddTokenAsync(Token token, CancellationToken cancellationToken);
    Task DeleteTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}