using ErrorOr;

namespace MovieMate.Api.Features.Identity;

public static class Errors
{
    public static class Authentication {
        
        public static Error InvalidCredentials => Error.Validation(
            code: "User.InvalidCredentials", 
            description: "Invalid credentials"
        );
        
        public static Error Unauthorized => Error.Unauthorized(
            code: "User.Unauthorized", 
            description: "Unauthorized"
        );

        public static Error InvalidToken => Error.NotFound(
            code: "User.InvalidToken", 
            description: "The token is invalid"
        );
        
        public static Error InvalidOperation => Error.Failure(
            code: "User.InvalidOperation", 
            description: "the operation is invalid"
        );
        
        public static Error RegistrationFailed(string? description)
        {
            return Error.Failure(
                code: "User.RegistrationFailed",
                description: description ?? "Registration failed."
            );
        }
    }
}
