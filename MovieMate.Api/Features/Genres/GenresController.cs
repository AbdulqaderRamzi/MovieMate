using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMate.Api.Controllers;
using MovieMate.Api.Features.Genres.Contracts;
using MovieMate.Api.Features.Genres.Services.IServices;
using MovieMate.Api.Features.Identity.Entities;

namespace MovieMate.Api.Features.Genres;

[Route("genres")]
[Authorize(Roles = Roles.Admin)]
public class GenresController : ApiController
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGenres()
    {
        return Ok(await _genreService.GetGenres());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGenreById(Guid id)
    {
        var getResult = await _genreService.GetGenreById(id);
        return getResult.Match(
            genre => Ok(genre),
            errors => Problem(errors)
            );
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre(CreateGenreRequest request)
    {
        var getResult = await _genreService.CreateGenre(request);
        return getResult.Match(
            genre => CreatedAtAction(nameof(GetGenreById),
                new { id = genre.Id }, genre),
            errors => Problem(errors)
        );
    }

    [HttpPut]
    public async Task<IActionResult> UpdateGenre(UpdateGenreRequest request)
    {
        var getResult = await _genreService.UpdateGenre(request);
        return getResult.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        var getResult = await _genreService.DeleteGenre(id);
        return getResult.Match(
            _ => NoContent(),
            errors => Problem(errors)
        );
    }
}