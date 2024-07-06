// Ignore Spelling: Validator

using FluentValidation;
using Management.Core.Entities;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using MassTransit;
using MediatR;
using Shared.IntegrationEvents;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Management.Core.Commands;

public static class UpdateRecipe
{
    public class UpdateRecipeRequest : IRequest
    {
        [JsonIgnore]
        public int Id { get; set; }

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

    public class Handler : IRequestHandler<UpdateRecipeRequest>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(IRecipeRepository recipeRepository, IPublishEndpoint publishEndpoint)
        {
            _recipeRepository = recipeRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(UpdateRecipeRequest request, CancellationToken cancellationToken)
        {
            Recipe currentEntity = (await _recipeRepository.GetById(request.Id))!;

            DateTime now = DateTime.UtcNow;
            DateTime? publishedAt = null;
            if (request.IsPublished)
            {
                if (currentEntity.IsPublished)
                {
                    publishedAt = currentEntity.PublishedAt;
                }
                else
                {
                    publishedAt = now;
                }
            }

            Recipe newEntity = new()
            {
                CategoryId = request.CategoryId,
                CookingMinutes = request.CookingMinutes,
                CreatedAt = currentEntity.CreatedAt,
                Description = request.Description,
                Id = request.Id,
                ImageFileName = request.ImageFileName,
                Ingredients = request.Ingredients,
                Instructions = request.Instructions,
                IsDeleted = false,
                IsPublished = request.IsPublished,
                PublishedAt = publishedAt,
                PreparationMinutes = request.PreparationMinutes,
                Title = request.Title,
                UpdatedAt = now,
                UserId = currentEntity.UserId,
            };

            await _recipeRepository.Update(newEntity);

            if (request.IsPublished)
            {
                if (!currentEntity.IsPublished)
                {
                    await _publishEndpoint.Publish(newEntity.ToRecipePublished());
                }
                else
                {
                    await _publishEndpoint.Publish(newEntity.ToRecipeUpdated());
                }
            }
            else
            {
                if (currentEntity.IsPublished)
                {
                    await _publishEndpoint.Publish(new RecipeUnpublished
                    {
                        Id = newEntity.Id,
                    });
                }
            }
        }
    }

    public class Validator : AbstractValidator<UpdateRecipeRequest>
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

            // check UserId
        }
    }
}
