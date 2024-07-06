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
        return _dbContext.Recipes.AnyAsync(x => x.CategoryId == id);
    }

    public async Task<int> Create(Category model)
    {
        _dbContext.Categories.Add(model);
        await _dbContext.SaveChangesAsync();
        return model.Id;
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
            query = query.Where(x => x.Name.ToUpper().Contains(name.ToUpper()));
        }

        return await query.ToArrayAsync();
    }

    public Task<Category?> GetById(int id)
    {
        return _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<Category>> GetByIds(IEnumerable<int> ids)
    {
        return _dbContext.Categories.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();
    }

    public Task<Category?> GetByName(string name)
    {
        return _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper());
    }

    public Task Update(Category model)
    {
        _dbContext.Categories.Update(model);
        return _dbContext.SaveChangesAsync();
    }
}
