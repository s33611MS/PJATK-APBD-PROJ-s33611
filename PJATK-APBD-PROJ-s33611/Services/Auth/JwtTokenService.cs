using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace PJATK_APBD_PROJ_s33611.Services.Auth;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(string userId, string username, string userRole)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, userRole)
        };

        var secretValue = configuration["JWT:Secret"]!;
        var symSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretValue));
        var signingCredentials = new SigningCredentials(symSecurityKey, SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("JWT:ExpirationMinutes")),
            Issuer = configuration["JWT:Issuer"],
            Audience = configuration["JWT:Audience"],
        });
        
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[96];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}