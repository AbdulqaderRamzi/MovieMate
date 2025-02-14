namespace MovieMate.Api.Features.Identity.Contracts;

public record AuthenticationResponse(
    string UserId,
    string FullName,
    string Email, 
    string Role,
    string AccessToken, 
    string RefreshToken
);