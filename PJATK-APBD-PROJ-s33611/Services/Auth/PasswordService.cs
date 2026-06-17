using Microsoft.AspNetCore.Identity;

namespace PJATK_APBD_PROJ_s33611.Services.Auth;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _hasher = new();
    
    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null, password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string password)
    {
        return _hasher.VerifyHashedPassword(null, hashedPassword, password) != PasswordVerificationResult.Failed;
    }
}