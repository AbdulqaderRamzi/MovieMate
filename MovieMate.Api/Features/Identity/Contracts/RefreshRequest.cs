namespace MovieMate.Api.Features.Identity.Contracts;

public record RefreshRequest(string AccessToken, string RefreshToken);