using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Api.Data.Entities;

namespace Notification.Api.Data.EntityConfigurations;

public class UserRecipeConfiguration : IEntityTypeConfiguration<UserRecipe>
{
    public void Configure(EntityTypeBuilder<UserRecipe> builder)
    {
        builder.ToTable("UserRecipe");

        builder.HasKey(x => new {x.UserId, x.RecipeId});
    }
}
