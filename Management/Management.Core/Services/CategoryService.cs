using FluentValidation;
using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Exceptions;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Management.Core.Services;

public class CategoryService : ICategoryService
{
    private readonly ILogger<CategoryService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private IValidator<CategoryCreateDto> _createValidator;
    private IValidator<CategoryUpdateDto> _updateValidator;

    public CategoryService(
        ILogger<CategoryService> logger,
        ICategoryRepository categoryRepository,
        IValidator<CategoryCreateDto> createValidator,
        IValidator<CategoryUpdateDto> updateValidator)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<CategoryDto>> Get(string? name = null)
    {
        IEnumerable<Category> categories = await _categoryRepository.Get(name);
        IEnumerable<CategoryDto> dtos = categories.Select(x => x.ToDto());
        return dtos;
    }

    public async Task<int> Create(CategoryCreateDto dto)
    {
        _createValidator.ValidateAndThrow(dto);

        Category? existingCategory = await _categoryRepository.GetByName(dto.Name);

        if (existingCategory is not null)
        {
            throw new BusinessLogicException($"The category name '{dto.Name}' has already existed.");
        }

        int id = await _categoryRepository.Create(dto);

        return id;
    }

    public async Task Update(int id, CategoryUpdateDto dto)
    {
        _updateValidator.ValidateAndThrow(dto);

        Category? categoryToUpdate = await _categoryRepository.GetById(id)
            ?? throw new NotFoundException($"No category with the given ID '{id}' found.");

        Category? existingCategory = await _categoryRepository.GetByName(dto.Name);
        if (existingCategory is not null && existingCategory.Id != id)
        {
            throw new BusinessLogicException($"The category name '{dto.Name}' has been used.");
        }

        categoryToUpdate.Name = dto.Name;
        await _categoryRepository.Update(categoryToUpdate);
    }

    public async Task Delete(int id)
    {
        Category? model = await _categoryRepository.GetById(id)
            ?? throw new NotFoundException($"No category with the given ID '{id}' found.");

        bool hasRecipes = await _categoryRepository.CheckCategoryHavingRecipes(id);
        if (hasRecipes)
        {
            throw new BusinessLogicException($"");
        }

        model.IsDeleted = true;
        await _categoryRepository.Update(model);
    }
}
