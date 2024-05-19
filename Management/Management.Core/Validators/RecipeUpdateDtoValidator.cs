﻿using FluentValidation;
using Management.Core.Dtos;
using Management.Core.Interfaces;

namespace Management.Core.Validators;

public class RecipeUpdateDtoValidator : AbstractValidator<RecipeUpdateDto>
{
    public RecipeUpdateDtoValidator(ICategoryRepository categoryRepository)
    {
        RuleFor(x => x.Ingredients)
            .NotEmpty();
        
        RuleFor(x => x.Instructions)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.PreparationMinutes)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CookingMinutes)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .MustAsync(async (categoryId, cancellationToken) =>
            {
                var category = await categoryRepository.GetById(categoryId);
                return category != null && !category.IsDeleted;
            }).WithMessage("Invalid category.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        // check UserId
    }
}
