using Management.Core.Dtos;
using Management.Core.Entities;

namespace Management.Core.Extensions;

public static class CategoryExtensions
{
    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
        };
    }
}
