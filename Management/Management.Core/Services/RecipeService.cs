using FluentValidation;
using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Exceptions;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.IntegrationEvents;

namespace Management.Core.Services;

public class RecipeService : IRecipeService
{
    private readonly ILogger<RecipeService> _logger;
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IValidator<RecipeCreateDto> _createValidator;
    private readonly IValidator<RecipePublishDto> _publishValidator;
    private readonly IValidator<RecipeUpdateDto> _updateValidator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUserRepository _userRepository;

    public RecipeService(
        ILogger<RecipeService> logger,
        IRecipeRepository recipeRepository,
        ICategoryRepository categoryRepository,
        IValidator<RecipeCreateDto> createValidator,
        IValidator<RecipePublishDto> publishValidator,
        IValidator<RecipeUpdateDto> updateValidator,
        IImageRepository imageRepository,
        IPublishEndpoint publishEndpoint,
        IUserRepository userRepository)
    {
        _logger = logger;
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _publishValidator = publishValidator;
        _updateValidator = updateValidator;
        _imageRepository = imageRepository;
        _publishEndpoint = publishEndpoint;
        _userRepository = userRepository;
    }

    public async Task<int> Create(RecipeCreateDto dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);
        string userId = await _userRepository.GetCurrentUserId();
        Recipe entity = dto.ToEntity(userId);
        int recipeId = await _recipeRepository.Create(entity);

        if (dto.IsPublished)
        {
            await _publishEndpoint.Publish(entity.ToRecipePublished());
        }

        return recipeId;
    }

    public async Task<PageResultDto<RecipeDto>> GetByCurrentUser(string? title = null, int pageNumber = 1, int pageSize = 10)
    {
        string currentUserId = await _userRepository.GetCurrentUserId();
        PageResultDto<Recipe> entityResult = await _recipeRepository.GetByUserId(currentUserId, title, pageNumber, pageSize);

        List<string> authorIds = entityResult.Items.Select(x => x.UserId).ToList();
        List<UserDto> users = await _userRepository.GetUsers(authorIds);

        List<int> categoryIds = entityResult.Items.Where(x => x.CategoryId.HasValue).Select(x => (int)x.CategoryId!).Distinct().ToList();
        List<Category> categories = await _categoryRepository.GetByIds(categoryIds);
        
        List<RecipeDto> dtos = entityResult.Items.Select(item =>
        {
            string? imageUrl = !string.IsNullOrEmpty(item.ImageFileName) ? _imageRepository.GetImageUrl(item.ImageFileName!) : null;
            Category? category = categories.Find(x => x.Id == item.CategoryId);
            UserDto? user = users.Find(u => u.Id == item.UserId);
            return item.ToDto(imageUrl, category?.Name, user);
        }).ToList();

        return new PageResultDto<RecipeDto>
        {
            Items = dtos,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = entityResult.TotalCount,
        };
    }

    public async Task<RecipeDto?> GetById(int id)
    {
        Recipe? entity = await _recipeRepository.GetById(id);
        if (entity == null)
        {
            return null;
        }

        string? imageUrl = !string.IsNullOrEmpty(entity.ImageFileName) ? _imageRepository.GetImageUrl(entity.ImageFileName!) : null;
        Category? category = entity.CategoryId.HasValue ? await _categoryRepository.GetById(entity.CategoryId.Value) : null;
        UserDto user = await _userRepository.GetUser(entity.UserId);
        return entity.ToDto(imageUrl, category?.Name, user);
    }

    public async Task Publish(RecipePublishDto dto)
    {
        await _publishValidator.ValidateAndThrowAsync(dto);

        Recipe? recipe = await _recipeRepository.GetById(dto.Id)
            ?? throw new NotFoundException($"No recipe with the given Id {dto.Id} found.");

        if (!recipe.IsPublished)
        {
            recipe.IsPublished = true;
            recipe.PublishedAt = DateTime.UtcNow;
            await _recipeRepository.Update(recipe);

            await _publishEndpoint.Publish(recipe.ToRecipePublished());
        }
    }

    public async Task Update(int id, RecipeUpdateDto dto)
    {
        await _updateValidator.ValidateAndThrowAsync(dto);

        Recipe currentEntity = (await _recipeRepository.GetById(id))!;

        DateTime now = DateTime.UtcNow;
        DateTime? publishedAt = null;
        if (dto.IsPublished)
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
            CategoryId = dto.CategoryId,
            CookingMinutes = dto.CookingMinutes,
            CreatedAt = currentEntity.CreatedAt,
            Description = dto.Description,
            Id = id,
            ImageFileName = dto.ImageFileName,
            Ingredients = dto.Ingredients,
            Instructions = dto.Instructions,
            IsDeleted = false,
            IsPublished = dto.IsPublished,
            PublishedAt = publishedAt,
            PreparationMinutes = dto.PreparationMinutes,
            Title = dto.Title,
            UpdatedAt = now,
            UserId = currentEntity.UserId,
        };

        await _recipeRepository.Update(newEntity);

        if (dto.IsPublished)
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
