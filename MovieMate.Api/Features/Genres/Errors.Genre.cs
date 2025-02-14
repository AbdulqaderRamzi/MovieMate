using ErrorOr;

namespace MovieMate.Api.Features.Genres;

public static class Errors
{
    public static class Genre
    {
        public static Error GenreNotFount => Error.NotFound(
            code: "User.GenreNotFount", 
            description: "The Genre was not found"
        );
        
        public static Error DuplicatedGenre => Error.Conflict(
            code: "User.DuplicatedGenre", 
            description: "A genre with this name already exists"
        );
    }
}