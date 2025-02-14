using MovieMate.Web.Contracts.Genre;
using MovieMate.Web.Models;

namespace MovieMate.Web.Services.IServices;

public interface IGenreService
{
    Task<ApiResponse<List<GenreResponse>>> GetGenres();
    Task<ApiResponse<GenreResponse>> GetGenreById(Guid id);
    Task<ApiResponse<GenreResponse>> CreateGenre(CreateGenreRequest request);
    Task<ApiResponse<object>> UpdateGenre(UpdateGenreRequest request);
    Task<ApiResponse<object>> DeleteGenre(Guid id);
}