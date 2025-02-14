using MovieMate.Api.Features.Movies.Entities;

namespace MovieMate.Api.Features.Movies.Services.IServices;

public interface IMovieService
{
    Task<List<Movie>> GetMovies();
    Task<Movie> GetMovieById(int id);
}