using Management.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Management.Infrastructure.EF.EntityConfigurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipe");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Instructions)
            .HasMaxLength(2000);

        builder.Property(x => x.Ingredients)
            .HasMaxLength(2000)
            .HasConversion(
                w => JsonConvert.SerializeObject(w),
                r => !string.IsNullOrEmpty(r)
                    ? JsonConvert.DeserializeObject<Collection<string>>(r) ?? new()
                    : new());
    }
}
