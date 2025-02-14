namespace MovieMate.Web.Services.IServices;

public interface ITokenService
{
    string? GetAccessToken();
    string? GetRefreshToken();
    void SetTokens(string accessToken, string refreshToken);
    void ClearTokens();
}