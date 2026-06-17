using Microsoft.EntityFrameworkCore;
using PJATK_APBD_PROJ_s33611.Data;
using PJATK_APBD_PROJ_s33611.Entities.Auth;

namespace PJATK_APBD_PROJ_s33611.Repositories.Auth;

public class AuthRepository(DatabaseContext ctx) : IAuthRepository
{
    public async Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await ctx.Users
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        return await ctx.Users
            .Include(u => u.UserRole)
            .Where(u => u.Token!.RefreshToken == refreshToken && u.Token.ExpiresAt >= DateTime.UtcNow)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Token?> GetTokenByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await ctx.Tokens.FirstOrDefaultAsync(t => t.UserId == id, cancellationToken);
    }

    public async Task<UserRole?> GetUserRoleAsync(CancellationToken cancellationToken)
    {
        return await ctx.UserRoles.FirstOrDefaultAsync(ur => ur.Name == "User", cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await ctx.Users.AddAsync(user, cancellationToken);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task AddTokenAsync(Token token, CancellationToken cancellationToken)
    {
        await ctx.Tokens.AddAsync(token, cancellationToken);
        await ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        await ctx.Tokens
            .Where(e => e.RefreshToken == refreshToken)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await ctx.SaveChangesAsync(cancellationToken);
    }
}