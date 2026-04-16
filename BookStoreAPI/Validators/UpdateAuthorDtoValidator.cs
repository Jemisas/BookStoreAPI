using BookStoreAPI.Models.DTOs;
using FluentValidation;

namespace BookStoreAPI.Validators;

public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
{
    public UpdateAuthorDtoValidator()
    {
        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .MinimumLength(2)
                .MaximumLength(200);
        });
    }
}
