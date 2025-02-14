using FluentValidation;
using MovieMate.Api.Features.Genres.Contracts;

namespace MovieMate.Api.Features.Genres.Validators;

public class CreateGenreValidator : AbstractValidator<CreateGenreRequest>
{
    public CreateGenreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Genre Name is required");
        
    }
}