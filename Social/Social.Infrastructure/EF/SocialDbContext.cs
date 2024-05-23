using Microsoft.EntityFrameworkCore;
using Social.Core.Entities;
using Social.Infrastructure.EF.EntityConfigurations;

namespace Social.Infrastructure.EF;

public class SocialDbContext : DbContext
{
    public DbSet<Like> Likes { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public SocialDbContext(DbContextOptions<SocialDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new LikeConfiguration().Configure(modelBuilder.Entity<Like>());
        new CommentConfiguration().Configure(modelBuilder.Entity<Comment>());
    }
}
