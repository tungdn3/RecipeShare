using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Api.Data.Entities;

namespace Notification.Api.Data.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.Property(x => x.DisplayName)
            .IsRequired();

        builder.HasMany<UserRecipe>()
            .WithOne(u => u.User)
            .HasPrincipalKey(u => u.Id);
    }
}
