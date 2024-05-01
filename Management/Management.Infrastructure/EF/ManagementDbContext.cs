using Management.Core.Entities;
using Management.Infrastructure.EF.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Management.Infrastructure.EF;

public class ManagementDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }

    public DbSet<Recipe> Recipes { get; set; }

    public ManagementDbContext(DbContextOptions<ManagementDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new CategoryConfiguration().Configure(modelBuilder.Entity<Category>());
        new RecipeConfiguration().Configure(modelBuilder.Entity<Recipe>());
    }
}
