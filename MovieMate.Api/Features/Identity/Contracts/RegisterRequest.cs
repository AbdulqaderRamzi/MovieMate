namespace MovieMate.Api.Features.Identity.Contracts;

public record RegisterRequest(
    string FirstName, 
    string LastName,
    string Email, 
    string Password, 
    string ConfirmPassword);
