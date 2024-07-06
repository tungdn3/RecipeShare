// Ignore Spelling: Validator

using FluentValidation;
using Management.Core.Entities;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using MassTransit;
using MediatR;

namespace Management.Core.Commands;

public static class PublishRecipe
{
    public class PublishRecipeRequest : IRequest
    {
        public int Id { get; set; }
    }

    public class Handler : IRequestHandler<PublishRecipeRequest>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(IRecipeRepository recipeRepository, IPublishEndpoint publishEndpoint)
        {
            _recipeRepository = recipeRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(PublishRecipeRequest request, CancellationToken cancellationToken)
        {
            Recipe recipe = (await _recipeRepository.GetById(request.Id))!;

            if (!recipe.IsPublished)
            {
                recipe.IsPublished = true;
                recipe.PublishedAt = DateTime.UtcNow;
                await _recipeRepository.Update(recipe);

                await _publishEndpoint.Publish(recipe.ToRecipePublished());
            }
        }

        public class Validator : AbstractValidator<PublishRecipeRequest>
        {
            public Validator(IRecipeRepository recipeRepository)
            {
                RuleFor(x => x.Id)
                    .NotEmpty()
                    .MustAsync(async (id, cancellationToken) =>
                    {
                        Recipe? recipe = await recipeRepository.GetById(id);
                        return recipe != null && !recipe.IsDeleted;
                    }).WithMessage("The recipe does not exist.");
            }
        }
    }
}
