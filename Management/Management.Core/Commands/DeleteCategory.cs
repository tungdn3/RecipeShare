using Management.Core.Entities;
using Management.Core.Exceptions;
using Management.Core.Interfaces;
using MediatR;

namespace Management.Core.Commands;

public static class DeleteCategory
{
    public class DeleteCategoryRequest : IRequest
    {
        public int Id { get; set; }
    }

    public class Handler : IRequestHandler<DeleteCategoryRequest>
    {
        private readonly ICategoryRepository _categoryRepository;

        public Handler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
        {
            Category? model = await _categoryRepository.GetById(request.Id)
                ?? throw new NotFoundException($"No category with the given ID '{request.Id}' found.");

            if (model.IsDeleted)
            {
                return;
            }

            bool hasRecipes = await _categoryRepository.CheckCategoryHavingRecipes(request.Id);
            if (hasRecipes)
            {
                throw new BusinessLogicException($"It is not allowed to delete a category having recipes.");
            }

            model.IsDeleted = true;
            await _categoryRepository.Update(model);
        }
    }
}
