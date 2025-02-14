using MovieMate.Web.Contracts.Genre;
using MovieMate.Web.Models;
using MovieMate.Web.Services.IServices;

namespace MovieMate.Web.Services;

public class GenreService : IGenreService
{
    private readonly IBaseApiClient _client;

    public GenreService(IBaseApiClient client)
    {
        _client = client;
    }
    
    public async Task<ApiResponse<List<GenreResponse>>> GetGenres()
    {
        return await _client.SendAsync<object, List<GenreResponse>>(
            HttpMethod.Get,
            "genres");
    }

    public async Task<ApiResponse<GenreResponse>> GetGenreById(Guid id)
    {
        return await _client.SendAsync<Guid, GenreResponse>(
            HttpMethod.Get,
            $"genres/{id}", 
            id);
    }

    public async Task<ApiResponse<GenreResponse>> CreateGenre(CreateGenreRequest request)
    {
        return await _client.SendAsync<CreateGenreRequest, GenreResponse>(
            HttpMethod.Post, 
            $"genres",
            request);
    }

    public async Task<ApiResponse<object>> UpdateGenre(UpdateGenreRequest request)
    {
        return await _client.SendAsync<UpdateGenreRequest, object>(
            HttpMethod.Put, 
            $"genres",
            request);
    }

    public async Task<ApiResponse<object>> DeleteGenre(Guid id)
    {
        return await _client.SendAsync<Guid, object>(
            HttpMethod.Delete, 
            $"genres/{id}",
            id);
    }
}