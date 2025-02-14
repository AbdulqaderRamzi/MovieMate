using MovieMate.Web.Services.IServices;

namespace MovieMate.Web.Services;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";

    public TokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetTokens(string accessToken, string refreshToken)
    {
        var options = GetCookieOptions();
        _httpContextAccessor.HttpContext?
            .Response.Cookies.Append(AccessTokenKey, accessToken, options);

        _httpContextAccessor.HttpContext?
            .Response.Cookies.Append(RefreshTokenKey, refreshToken, options);
    }
    
    public string? GetAccessToken()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Cookies[AccessTokenKey];
    }

    public string? GetRefreshToken()
    {
        return _httpContextAccessor.HttpContext?
            .Request.Cookies[RefreshTokenKey];
    }

    public void ClearTokens()
    {
        _httpContextAccessor.HttpContext?
            .Response.Cookies.Delete(AccessTokenKey);

        _httpContextAccessor.HttpContext?
            .Response.Cookies.Delete(RefreshTokenKey);
    }

    private CookieOptions GetCookieOptions()
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }
}