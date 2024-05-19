using FluentValidation;
using Management.Core.Interfaces;

namespace Management.Core.Validators;

public class CategoryDeleteValidator : AbstractValidator<int>
{
    public CategoryDeleteValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(id => id)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
            {
                bool exist = await categoryRepository.Exists(id);
                return !exist;
            }).WithMessage("No category with the given Id found.")
            .MustAsync(async (id, cancellation) =>
            {
                bool hasRecipes = await categoryRepository.CheckCategoryHavingRecipes(id);
                return !hasRecipes;
            }).WithMessage("It is not allowed to delete a category having recipes.");
    }
}
