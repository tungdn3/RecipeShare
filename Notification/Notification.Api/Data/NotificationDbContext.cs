using Microsoft.EntityFrameworkCore;
using Notification.Api.Data.Entities;
using Notification.Api.Data.EntityConfigurations;

namespace Notification.Api.Data;

public class NotificationDbContext : DbContext
{
    public DbSet<NotificationMessage> NotificationMessages { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<UserRecipe> UserRecipes { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new NotificationMessageConfiguration().Configure(modelBuilder.Entity<NotificationMessage>());
        new UserConfiguration().Configure(modelBuilder.Entity<User>());
        new UserRecipeConfiguration().Configure(modelBuilder.Entity<UserRecipe>());
    }
}
