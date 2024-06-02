using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Api.Data.Entities;

namespace Notification.Api.Data.EntityConfigurations;

public class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
{
    public void Configure(EntityTypeBuilder<NotificationMessage> builder)
    {
        builder.ToTable("NotificationMessage");

        builder.Property(x => x.RecipeId)
            .IsRequired();

        builder.HasOne(x => x.FromUser)
            .WithMany()
            .HasForeignKey(n => n.FromUserId)
            .IsRequired();

        builder.Property(x => x.ToUserId)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion(
                v => v.ToString(),
                v => (NotificationType)Enum.Parse(typeof(NotificationType), v));
    }
}
