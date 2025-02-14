using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using MovieMate.Web.Contracts.Identity;
using MovieMate.Web.Models;
using MovieMate.Web.Services.IServices;

namespace MovieMate.Web.Services;

public class AuthService : IAuthService
{
    private readonly IBaseApiClient _client;

    public AuthService(IBaseApiClient client)
    {
        _client = client;
    }

    public async Task<ApiResponse<AuthenticationResponse>> LoginAsync(LoginRequest request)
    {
        return await _client.SendAsync<LoginRequest, AuthenticationResponse>(
            HttpMethod.Post, 
            "auth/login",
            request, 
            false
            );
    }

    public async Task<ApiResponse<AuthenticationResponse>> RegisterAsync(RegisterRequest request)
    {
        return await _client.SendAsync<RegisterRequest, AuthenticationResponse>(
            HttpMethod.Post, 
            "auth/register",
            request, 
            false
        );
    }

    public ClaimsPrincipal AddClaims(AuthenticationResponse response)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, response.FullName));
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, response.Email));
        identity.AddClaim(new Claim(ClaimTypes.Email, response.Email));
        identity.AddClaim(new Claim(ClaimTypes.Role, response.Role));
        return new ClaimsPrincipal(identity);
    }
}