namespace MovieMate.Api.Features.Genres.Contracts;

public record GenreResponse(
    Guid Id,
    string Name); // You can add movies related to a genre 