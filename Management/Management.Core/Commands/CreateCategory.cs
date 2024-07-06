// Ignore Spelling: Validator

using FluentValidation;
using Management.Core.Entities;
using Management.Core.Exceptions;
using Management.Core.Interfaces;
using MediatR;

namespace Management.Core.Commands;

public static class CreateCategory
{
    public class CreateCategoryRequest : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<CreateCategoryRequest, int>
    {
        private readonly ICategoryRepository _categoryRepository;

        public Handler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<int> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            Category? existingCategory = await _categoryRepository.GetByName(request.Name);

            if (existingCategory is not null)
            {
                throw new BusinessLogicException($"The category name '{request.Name}' has already existed.");
            }

            Category category = new()
            {
                Name = request.Name,
                IsDeleted = false,
            };

            int id = await _categoryRepository.Create(category);

            return id;
        }
    }

    public class Validator : AbstractValidator<CreateCategoryRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
