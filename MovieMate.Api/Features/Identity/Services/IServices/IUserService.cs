using ErrorOr;
using MovieMate.Api.Features.Identity.Contracts;
using MovieMate.Api.Features.Identity.Entities;

namespace MovieMate.Api.Features.Identity.Services.IServices;

public interface IUserService
{
    Task<ErrorOr<AuthenticationResponse>> Register(ApplicationUser user, string password);
    Task<ErrorOr<AuthenticationResponse>> Login(LoginRequest request);
    Task<ErrorOr<AuthenticationResponse>> Refresh(RefreshRequest request);
    Task<ErrorOr<Success>> Revoke(Guid userId);
}