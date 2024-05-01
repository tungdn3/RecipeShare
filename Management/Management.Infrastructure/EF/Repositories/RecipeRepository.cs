using Management.Core.Entities;
using Management.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Management.Infrastructure.EF.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly ManagementDbContext _context;

    public RecipeRepository(ManagementDbContext context)
    {
        _context = context;
    }

    public async Task<int> Create(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
        return recipe.Id;
    }

    public async Task<Recipe?> GetById(int id)
    {
        Recipe? recipe = await _context.Recipes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return recipe;
    }

    public Task<Recipe[]> GetByUserId(string userId, string? title)
    {
        var query = _context.Recipes
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrEmpty(title))
        {
            query = query.Where(x => x.Title.Contains(title));
        }

        return query.ToArrayAsync();
    }

    public Task Update(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
        return _context.SaveChangesAsync();
    }
}
