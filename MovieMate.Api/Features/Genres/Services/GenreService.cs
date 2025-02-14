using ErrorOr;
using Microsoft.EntityFrameworkCore;
using MovieMate.Api.Features.Genres.Contracts;
using MovieMate.Api.Features.Genres.Entities;
using MovieMate.Api.Features.Genres.Services.IServices;

namespace MovieMate.Api.Features.Genres.Services;

public class GenreService : IGenreService
{
    private readonly ApplicationDbContext _db;

    public GenreService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<GenreResponse>> GetGenres()
    {
        return await _db.Genres
            .Select(genre => new GenreResponse(genre.Id, genre.Name))
            .ToListAsync();
    }

    public async Task<ErrorOr<GenreResponse>> GetGenreById(Guid id)
    {
        var response = await _db.Genres.FirstOrDefaultAsync(genre => genre.Id == id);
        if (response is null)
        {
            return Errors.Genre.GenreNotFount;
        }
        return new GenreResponse(response.Id, response.Name);
    }

    public async Task<ErrorOr<GenreResponse>> CreateGenre(CreateGenreRequest request)
    {
        var genreName = request.Name.Trim().ToLower();
    
        if (await _db.Genres.AnyAsync(genre => genre.Name == genreName))
        {
            return Errors.Genre.DuplicatedGenre;
        }
       
        var genre = new Genre { Name = request.Name };
        await _db.Genres.AddAsync(genre);
        await _db.SaveChangesAsync();
        
        return new GenreResponse(genre.Id, genre.Name);
    }

    public async Task<ErrorOr<Success>> UpdateGenre(UpdateGenreRequest request)
    {
        if (await _db.Genres.FirstOrDefaultAsync(g => g.Id == request.Id) is not { } genre)
        {
            return Errors.Genre.GenreNotFount;
        }
        var genreName = request.Name.Trim().ToLower();
        if (await _db.Genres.Where(g => g.Id != request.Id).AnyAsync(g => g.Name == genreName))
        {
            return Errors.Genre.DuplicatedGenre;
        }
        var newName = request.Name.Trim().ToLower();
        genre.Name = newName;
        await _db.SaveChangesAsync();
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> DeleteGenre(Guid id)
    {
        if (await _db.Genres.FirstOrDefaultAsync(g => g.Id == id) is not { } genre)
        {
            return Errors.Genre.GenreNotFount;
        }
        _db.Genres.Remove(genre);
        await _db.SaveChangesAsync();
        return Result.Success;
    }
}