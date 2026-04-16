using BookStoreAPI.Models.DTOs;
using FluentValidation;

namespace BookStoreAPI.Validators;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Isbn)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(300);

        RuleFor(x => x.PublicationYear)
            .InclusiveBetween(1000, DateTime.UtcNow.Year);

        RuleFor(x => x.AuthorId)
            .NotEmpty();
    }
}
