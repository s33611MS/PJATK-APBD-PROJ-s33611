using Microsoft.AspNetCore.Mvc;
using PJATK_APBD_PROJ_s33611.DTOs.Auth;
using PJATK_APBD_PROJ_s33611.Services.Auth;

namespace PJATK_APBD_PROJ_s33611.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto request, CancellationToken cancellationToken)
    {
        var result = await service.SignUpAsync(request, cancellationToken);

        AppendRefreshTokenCookie(HttpContext, result.RefreshToken);
        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost]
    [Route("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequestDto request, CancellationToken cancellationToken)
    {
        var result = await service.SignInAsync(request, cancellationToken);

        AppendRefreshTokenCookie(HttpContext, result.RefreshToken);
        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = GetRefreshTokenFromCookie(HttpContext);
        if (refreshToken is null)
            return Unauthorized();

        var result = await service.RefreshAsync(refreshToken, cancellationToken);

        AppendRefreshTokenCookie(HttpContext, result.RefreshToken);
        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost]
    [Route("sign-out")]
    public async Task<IActionResult> SignOut(CancellationToken cancellationToken)
    {
        var refreshToken = GetRefreshTokenFromCookie(HttpContext);
        if (refreshToken is null)
            return NoContent();

        await service.SignOutAsync(refreshToken, cancellationToken);

        RemoveRefreshTokenCookie(HttpContext);
        return NoContent();
    }
    private static string? GetRefreshTokenFromCookie(HttpContext httpContext)
    {
        return httpContext.Request.Cookies["ref-token"];
    }
    
    private static void AppendRefreshTokenCookie(HttpContext httpContext, string refreshToken)
    {
        httpContext.Response.Cookies.Append("ref-token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private static void RemoveRefreshTokenCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("ref-token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    }
}