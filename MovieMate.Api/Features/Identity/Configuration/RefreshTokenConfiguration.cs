using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieMate.Api.Features.Identity.Entities;

namespace MovieMate.Api.Features.Identity.Configuration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Token).IsUnique();
        builder.Property(x => x.Token).HasMaxLength(200);
        
        builder.HasOne(x => x.User)
            .WithMany().HasForeignKey(x => x.UserId);

    }
}