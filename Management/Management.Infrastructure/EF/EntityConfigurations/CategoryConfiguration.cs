using Management.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Management.Infrastructure.EF.EntityConfigurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired();

        builder.HasMany<Recipe>()
            .WithOne();
    }
}
