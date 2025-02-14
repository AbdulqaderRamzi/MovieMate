using ErrorOr;
using MovieMate.Api.Features.Genres.Contracts;

namespace MovieMate.Api.Features.Genres.Services.IServices;

public interface IGenreService
{
    Task<List<GenreResponse>> GetGenres();
    Task<ErrorOr<GenreResponse>> GetGenreById(Guid id);
    Task<ErrorOr<GenreResponse>> CreateGenre(CreateGenreRequest request);
    Task<ErrorOr<Success>> UpdateGenre(UpdateGenreRequest request);
    Task<ErrorOr<Success>> DeleteGenre(Guid id);
}