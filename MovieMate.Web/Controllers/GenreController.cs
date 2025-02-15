using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMate.Web.Services.IServices;

namespace MovieMate.Web.Controllers;

[Authorize]
public class GenreController : Controller
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    public async Task<IActionResult> Index()
    {
        var response = await _genreService.GetGenres();
        if (response.IsSuccess) return View(response.Data);
        return RedirectToAction("Index", "Home");
    }
}