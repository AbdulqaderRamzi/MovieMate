using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieMate.Api.Features.Identity.Contracts;
using MovieMate.Api.Features.Identity.Entities;
using MovieMate.Api.Features.Identity.Services.IServices;


namespace MovieMate.Api.Features.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _context;
    private readonly TokenValidationParameters _tokenValidation;
    
    public UserService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext db,
        IHttpContextAccessor context, TokenValidationParameters tokenValidation)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _roleManager = roleManager;
        _db = db;
        _context = context;
        _tokenValidation = tokenValidation;
    }

    public async Task<ErrorOr<AuthenticationResponse>> Register(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return Errors.Authentication.RegistrationFailed(result.Errors.FirstOrDefault()?.Description);
        }

        if (!await _roleManager.RoleExistsAsync("admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            await _roleManager.CreateAsync(new IdentityRole("visitor"));
        }

        await _userManager.AddToRoleAsync(user, Roles.Visitor); // example

        var jti = Guid.NewGuid().ToString();
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, Roles.Visitor, jti);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _jwtTokenGenerator.GenerateRefreshToken(),
            CreatedOnUtc = DateTime.UtcNow,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
            JwtId = jti
        };
        await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();
        
        return new AuthenticationResponse(
            UserId: user.Id,
            FullName: $"{user.FirstName} {user.LastName}",
            Email: user.UserName!, 
            Role: Roles.Visitor, 
            accessToken, 
            refreshToken.Token);
    }

    public async Task<ErrorOr<AuthenticationResponse>> Login(LoginRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Errors.Authentication.InvalidCredentials;
        }

        var role = _userManager.GetRolesAsync(user).Result.First();
        var jti = Guid.NewGuid().ToString();
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, role, jti);
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = _jwtTokenGenerator.GenerateRefreshToken(),
            CreatedOnUtc = DateTime.UtcNow,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
            JwtId = jti
        };
        await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();
        
        return new AuthenticationResponse(
            UserId: user.Id,
            FullName: $"{user.FirstName} {user.LastName}",
            Email: user.UserName!, 
            Role: role, 
            accessToken, 
            refreshToken.Token);
    }

    public async Task<ErrorOr<Success>> Revoke(Guid userId)
    {
        var currentUserId = _context.HttpContext?.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Errors.Authentication.Unauthorized;
        }

        if (userId.ToString() != currentUserId)
        {
            return Errors.Authentication.InvalidOperation;
        }

        await _db.RefreshTokens.Where(rt => rt.UserId == userId.ToString())
            .ExecuteDeleteAsync();
        return Result.Success;
    }

    
    public async Task<ErrorOr<AuthenticationResponse>> Refresh(RefreshRequest request)
    {
        var refreshToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);
        
        if (refreshToken is null)
        {
            return Errors.Authentication.InvalidToken;
        }
        if (!refreshToken.IsValid)
        {
            await _db.RefreshTokens
                .Where(rt => rt.UserId == refreshToken.UserId && rt.JwtId == refreshToken.JwtId)
                .ExecuteUpdateAsync(rt => rt.SetProperty(p => p.IsValid, false));
            return Errors.Authentication.InvalidToken;
        }
        var claims = GetClaimsFromToken(request.AccessToken);
        if (claims is null)
        {
            refreshToken.IsValid = false;
            await _db.SaveChangesAsync();
            return Errors.Authentication.InvalidToken;
        }
        
        var jti = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)!.Value;
        var userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value;
        if (refreshToken.ExpiresOnUtc < DateTime.UtcNow || 
            refreshToken.JwtId != jti ||
            refreshToken.UserId != userId)
        {
            refreshToken.IsValid = false;
            await _db.SaveChangesAsync();
            return Errors.Authentication.InvalidToken;
        }
        refreshToken.IsValid = false;
        var newRefreshToken = new RefreshToken
        {
            UserId = userId,
            Token = _jwtTokenGenerator.GenerateRefreshToken(), CreatedOnUtc = DateTime.UtcNow,
            ExpiresOnUtc = DateTime.UtcNow.AddMinutes(2),
            JwtId = jti
        };
         await _db.RefreshTokens.AddAsync(newRefreshToken);
        await _db.SaveChangesAsync();
        
        var role = _userManager.GetRolesAsync(refreshToken.User!).Result.First();
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(refreshToken.User!, role, jti);
        
        return new AuthenticationResponse(
            UserId: refreshToken.User!.Id,
            FullName: $"{refreshToken.User!.FirstName} {refreshToken.User!.LastName}",
            Email: refreshToken.User!.UserName!, 
            Role: role, 
            accessToken, 
            newRefreshToken.Token);
    }
    
    private List<Claim>? GetClaimsFromToken(string accessToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(accessToken);
            return jwt.Claims.ToList();
        }
        catch
        {
            return null;
        }

        /*var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal =  tokenHandler
                .ValidateToken(accessToken, _tokenValidation, out var validatedToken);
            return !IsJwtWithValidSecureAlgorithm(validatedToken) ? null : principal;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }*/
    }

    /*private bool IsJwtWithValidSecureAlgorithm(SecurityToken validatedToken)
    {
        return validatedToken is JwtSecurityToken jwtSecurityToken && 
               jwtSecurityToken.Header.Alg.Equals(
                   SecurityAlgorithms.HmacSha256,
                   StringComparison.InvariantCultureIgnoreCase);
    }*/
}