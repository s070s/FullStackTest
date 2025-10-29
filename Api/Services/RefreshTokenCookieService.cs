
using Microsoft.AspNetCore.Http;
namespace Api.Services
{
    public interface IRefreshTokenCookieService
    {
        CookieOptions CreateRefreshTokenCookieOptions(HttpContext context, DateTime? expiresUtc);
        void SetRefreshToken(HttpContext context, string token, DateTime? expiresUtc);
        void RemoveRefreshToken(HttpContext context);
        bool IsHttpsRequest(HttpContext context);
    }


    public class RefreshTokenCookieService : IRefreshTokenCookieService
{
    public CookieOptions CreateRefreshTokenCookieOptions(HttpContext context, DateTime? expiresUtc)
    {
        var isHttps = IsHttpsRequest(context);
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = expiresUtc,
            Path = "/"
        };
    }

    public void SetRefreshToken(HttpContext context, string token, DateTime? expiresUtc)
    {
        var options = CreateRefreshTokenCookieOptions(context, expiresUtc);
        context.Response.Cookies.Append("refreshToken", token, options);
    }

    public void RemoveRefreshToken(HttpContext context)
    {
        var options = CreateRefreshTokenCookieOptions(context, DateTime.UtcNow.AddDays(-1));
        context.Response.Cookies.Append("refreshToken", "", options);
    }

    public bool IsHttpsRequest(HttpContext context)
    {
        return context.Request.IsHttps ||
               string.Equals(context.Request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase);
    }
}

}
