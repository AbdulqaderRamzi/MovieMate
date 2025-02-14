namespace MovieMate.Web.Contracts.Identity;

public record RegisterRequest(
    string FirstName, 
    string LastName,
    string Email, 
    string Password, 
    string ConfirmPassword);
