using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieMate.Api.Features.Genres.Entities;

namespace MovieMate.Api.Features.Genres.Configuration;

public sealed class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.HasKey(g => g.Id);
        builder.HasIndex(g => g.Name).IsUnique();
    }
}