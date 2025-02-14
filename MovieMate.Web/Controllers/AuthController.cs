using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MovieMate.Web.Contracts.Identity;
using MovieMate.Web.Services.IServices;

namespace MovieMate.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }
    
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        if (!response.IsSuccess)
        {
            ModelState.AddModelError("", response.Error!.Title!);
            return View(request);
        }
        var claimsPrincipal = _authService.AddClaims(response.Data!);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal);
        _tokenService.SetTokens(response.Data!.AccessToken, response.Data!.RefreshToken);
        return RedirectToAction(nameof(Index), "Home");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (!response.IsSuccess)
        {
            ModelState.AddModelError("", response.Error!.Title!);
            return View(request);
        }
        var claimsPrincipal = _authService.AddClaims(response.Data!);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal); 
        _tokenService.SetTokens(response.Data!.AccessToken, response.Data!.RefreshToken);
        return RedirectToAction(nameof(Index), "Home");
    }
    
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync();
        _tokenService.ClearTokens();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
} 