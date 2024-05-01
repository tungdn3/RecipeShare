using FluentValidation;
using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Interfaces;

namespace Management.Core.Validators;

public class RecipePublishDtoValidator : AbstractValidator<RecipePublishDto>
{
    public RecipePublishDtoValidator(IRecipeRepository recipeRepository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellationToken) =>
            {
                Recipe recipe = await recipeRepository.GetById(id);
                return recipe != null && !recipe.IsDeleted;
            }).WithMessage("The recipe does not exist.");
    }
}
