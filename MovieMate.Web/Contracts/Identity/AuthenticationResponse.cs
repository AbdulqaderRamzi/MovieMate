namespace MovieMate.Web.Contracts.Identity;

public record AuthenticationResponse(
    string UserId,
    string FullName,
    string Email, 
    string Role,
    string AccessToken, 
    string RefreshToken
);