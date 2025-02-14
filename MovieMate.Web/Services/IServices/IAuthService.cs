using System.Security.Claims;
using MovieMate.Web.Contracts.Identity;
using MovieMate.Web.Models;

namespace MovieMate.Web.Services.IServices;

public interface IAuthService
{
    Task<ApiResponse<AuthenticationResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthenticationResponse>> RegisterAsync(RegisterRequest request);
    ClaimsPrincipal AddClaims(AuthenticationResponse response);
}