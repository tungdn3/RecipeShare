using FluentValidation;
using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Management.Core.Services;

public class RecipeService : IRecipeService
{
    private readonly ILogger<RecipeService> _logger;
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<RecipeCreateDto> _createValidator;
    private readonly IValidator<RecipePublishDto> _publishValidator;
    private readonly IValidator<RecipeUpdateDto> _updateValidator;

    public RecipeService(
        ILogger<RecipeService> logger,
        IRecipeRepository recipeRepository,
        ICategoryRepository categoryRepository,
        IValidator<RecipeCreateDto> createValidator,
        IValidator<RecipePublishDto> publishValidator,
        IValidator<RecipeUpdateDto> updateValidator)
    {
        _logger = logger;
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _publishValidator = publishValidator;
        _updateValidator = updateValidator;
    }

    public async Task<int> Create(string userId, RecipeCreateDto dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);
        Recipe entity = dto.ToEntity(userId);
        int recipeId = await _recipeRepository.Create(entity);

        if (dto.IsPublished)
        {
            // Send published event, or created event
        }

        return recipeId;
    }

    public async Task<RecipeDto[]> Get(string userId, string? title = null)
    {
        Recipe[] entities = await _recipeRepository.GetByUserId(userId, title);
        RecipeDto[] dtos = entities.Select(x => x.ToDto()).ToArray();
        return dtos;
    }

    public async Task<RecipeDto?> GetById(string userId, int id)
    {
        Recipe? entity = await _recipeRepository.GetById(id);
        return entity?.ToDto();
    }

    public async Task Publish(RecipePublishDto dto)
    {
        await _publishValidator.ValidateAndThrowAsync(dto);

        Recipe? recipe = await _recipeRepository.GetById(dto.Id);
        if (!recipe.IsPublished)
        {
            recipe.IsPublished = true;
            recipe.PublishedAt = DateTime.UtcNow;
            await _recipeRepository.Update(recipe);

            // send published event
        }
    }

    public async Task Update(int id, RecipeUpdateDto dto)
    {
        await _updateValidator.ValidateAndThrowAsync(dto);

        Recipe? currentEntity = await _recipeRepository.GetById(id);
        
        DateTime now = DateTime.UtcNow;
        bool isPublishedNow = dto.IsPublished && !currentEntity.IsPublished;
        
        Recipe newEntity = new()
        {
            CategoryId = dto.CategoryId,
            CookingMinutes = dto.CookingMinutes,
            CreatedAt = currentEntity.CreatedAt,
            Description = dto.Description,
            Id = id,
            ImageUrl = dto.ImageUrl,
            Ingredients = dto.Ingredients,
            Instructions = dto.Instructions,
            IsDeleted = false,
            IsPublished = dto.IsPublished,
            PublishedAt = isPublishedNow ? now : currentEntity.PublishedAt,
            PreparationMinutes = dto.PreparationMinutes,
            Title = dto.Title,
            UpdatedAt = now,
            UserId = currentEntity.UserId,
        };
        
        await _recipeRepository.Update(newEntity);
        bool isUnpublished = currentEntity.IsPublished && !dto.IsPublished;
        if (isPublishedNow)
        {
            // send published event
        }
        else if (isUnpublished)
        {
            // send Unpublished event
        }
    }
}
