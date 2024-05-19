using FluentValidation;
using Management.Core.Dtos;

namespace Management.Core.Validators;

public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator()
    {
        
    }
}
