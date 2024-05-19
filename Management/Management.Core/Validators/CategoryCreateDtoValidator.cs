// Ignore Spelling: Validator

using FluentValidation;
using Management.Core.Dtos;

namespace Management.Core.Validators;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
