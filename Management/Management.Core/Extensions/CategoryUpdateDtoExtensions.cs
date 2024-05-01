using Management.Core.Dtos;
using Management.Core.Entities;

namespace Management.Core.Extensions;

public static class CategoryUpdateDtoExtensions
{
    public static Category ToModel(this CategoryUpdateDto dto)
    {
        return new Category
        {
            Name = dto.Name,
        };
    }
}
