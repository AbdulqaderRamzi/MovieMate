using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieMate.Api.Controllers;
using MovieMate.Api.Features.Identity.Contracts;
using MovieMate.Api.Features.Identity.Entities;
using MovieMate.Api.Features.Identity.Services.IServices;

namespace MovieMate.Api.Features.Identity;

[Route("auth")]
[AllowAnonymous]
public class AuthenticationController : ApiController
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    
    public AuthenticationController(
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator,
        IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = _mapper.Map<ApplicationUser>(request);
        var authResult = await _userService.Register(user, request.Password);
        return authResult.Match(
            result => Ok(result),
            error => Problem(error)
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var authResult = await _userService.Login(request);
        return authResult.Match(
            result => Ok(result),
            error => Problem(error)
        );
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var authResult = await _userService.Refresh(request);
        return authResult.Match(
            result => Ok(result),
            error => Problem(error)
        );
    }
    
    [HttpDelete("revoke/{userId:guid}")]
    public async Task<IActionResult> Revoke(Guid userId)
    {
        var authResult = await _userService.Revoke(userId);
        
        return authResult.Match(
            result => NoContent(),
            errors => Problem(errors)
        );
    }
}