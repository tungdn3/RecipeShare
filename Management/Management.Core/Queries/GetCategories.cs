using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using MediatR;

namespace Management.Core.CategoryUseCases.Queries;

public static class GetCategories
{
    public class GetCategoriesRequest : IRequest<List<CategoryDto>>
    {
        public string? Name { get; set; }
    }

    public class Handler : IRequestHandler<GetCategoriesRequest, List<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public Handler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Category> categories = await _categoryRepository.Get(request.Name);
            List<CategoryDto> dtos = categories.Select(x => x.ToDto()).ToList();
            return dtos;
        }
    }
}
