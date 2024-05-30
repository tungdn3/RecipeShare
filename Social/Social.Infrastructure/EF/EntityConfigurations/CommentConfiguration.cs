using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social.Core.Entities;

namespace Social.Infrastructure.EF.EntityConfigurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comment");

        builder.HasOne<Comment>()
            .WithMany();

        builder.Property(x => x.RecipeId)
            .IsRequired();

        builder.HasIndex(x => x.RecipeId);

        builder.HasIndex(x => x.ParentId);
    }
}
