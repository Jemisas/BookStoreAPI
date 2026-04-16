using BookStoreAPI.Models.DTOs;
using FluentValidation;

namespace BookStoreAPI.Validators;

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        When(x => x.Title is not null, () =>
        {
            RuleFor(x => x.Title!)
                .MaximumLength(300);
        });

        When(x => x.PublicationYear.HasValue, () =>
        {
            RuleFor(x => x.PublicationYear!.Value)
                .InclusiveBetween(1000, DateTime.UtcNow.Year);
        });
    }
}
