// Ignore Spelling: Validator

using FluentValidation;
using Management.Core.Entities;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using MassTransit;
using MediatR;
using System.Collections.ObjectModel;

namespace Management.Core.Commands;

public static class CreateRecipe
{
    public class CreateRecipeRequest : IRequest<int>
    {
        public int? CategoryId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ImageFileName { get; set; }

        public int PreparationMinutes { get; set; }

        public int CookingMinutes { get; set; }

        public Collection<string> Ingredients { get; set; } = [];

        public string Instructions { get; set; } = string.Empty;

        public bool IsPublished { get; set; }
    }

    public class Handler : IRequestHandler<CreateRecipeRequest, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(IUserRepository userRepository, IRecipeRepository recipeRepository, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> Handle(CreateRecipeRequest request, CancellationToken cancellationToken)
        {
            string userId = await _userRepository.GetCurrentUserId();
            Recipe entity = request.ToEntity(userId);
            int recipeId = await _recipeRepository.Create(entity);

            if (request.IsPublished)
            {
                await _publishEndpoint.Publish(entity.ToRecipePublished());
            }

            return recipeId;
        }
    }

    public class Validator : AbstractValidator<CreateRecipeRequest>
    {
        public Validator(ICategoryRepository categoryRepository)
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
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    if (!categoryId.HasValue)
                    {
                        return true;
                    }

                    var category = await categoryRepository.GetById(categoryId.Value);
                    return category != null && !category.IsDeleted;
                }).WithMessage("Invalid category.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}
