namespace MovieMate.Api.Features.Identity.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = null!;
    public string JwtId { get; set; } = null!;
    public DateTime CreatedOnUtc { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
    public bool IsValid { get; set; } = true;
    public string UserId { get; set; } = null!;
    public ApplicationUser? User { get; set; }
}