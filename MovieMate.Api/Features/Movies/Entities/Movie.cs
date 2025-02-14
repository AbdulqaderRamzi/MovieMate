namespace MovieMate.Api.Features.Movies.Entities;

public class Movie
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public string Synopsis { get; set; } = null!;
    public int Runtime  { get; set; } // duration of the movie in minutes.
    public string PosterUrl { get; set; } = null!;
    public float? Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}