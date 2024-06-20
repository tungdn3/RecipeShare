using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Management.Infrastructure.EF.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ManagementDbContext _dbContext;

    public CategoryRepository(ManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> CheckCategoryHavingRecipes(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Create(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            IsDeleted = false,
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();
        return category.Id;
    }

    public Task<bool> Exists(int id)
    {
        return _dbContext.Categories.AnyAsync(x => x.Id == id);
    }

    public async Task<Category[]> Get(string? name = null)
    {
        IQueryable<Category> query = _dbContext.Categories.AsNoTracking();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        return await query.ToArrayAsync();
    }

    public Task<Category?> GetById(int id)
    {
        return _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<Category?> GetByName(string name)
    {
        return _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);
    }

    public Task Update(Category model)
    {
        throw new NotImplementedException();
    }
}
