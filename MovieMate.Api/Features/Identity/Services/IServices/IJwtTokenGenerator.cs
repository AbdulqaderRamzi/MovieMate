using MovieMate.Api.Features.Identity.Entities;

namespace MovieMate.Api.Features.Identity.Services.IServices;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(ApplicationUser user, string role, string jti);
    string GenerateRefreshToken();
}