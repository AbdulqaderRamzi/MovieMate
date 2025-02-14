using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieMate.Api.Features.Genres.Services;
using MovieMate.Api.Features.Genres.Services.IServices;
using MovieMate.Api.Features.Identity.Entities;
using MovieMate.Api.Features.Identity.Services;
using MovieMate.Api.Features.Identity.Services.IServices;

namespace MovieMate.Api;

public static class DependencyInjection
{
    public static void AddApplicationServices(
        this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddControllers();
        services.AddInfrastructure(configuration);
        services.AddMappings();
        services.AddValidators();

        services.AddScoped<IGenreService, GenreService>();
    }

    private static void AddInfrastructure(
        this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
        
        var jwtSettings = new JwtSettings();   
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserService, UserService>();
        
        services
            //.AddIdentityApiEndpoints<ApplicationUser>()
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            /*.AddDefaultTokenProviders();*/

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)
            )
        };
        
       services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options => options.TokenValidationParameters = tokenValidationParameters);
       
        services.AddSingleton(tokenValidationParameters);
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });
    }

    private static void AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            includeInternalTypes: true);
        services.AddFluentValidationAutoValidation();
    }
}