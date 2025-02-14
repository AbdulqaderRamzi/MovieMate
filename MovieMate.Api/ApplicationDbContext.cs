using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieMate.Api.Features.Genres.Entities;
using MovieMate.Api.Features.Identity.Entities;
using MovieMate.Api.Features.Movies.Entities;

namespace MovieMate.Api;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext(options)
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<RefreshToken> RefreshTokens{ get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); 
    }
}