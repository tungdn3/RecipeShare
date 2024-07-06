// Ignore Spelling: Validator

using FluentValidation;
using Management.Core.Entities;
using Management.Core.Exceptions;
using Management.Core.Interfaces;
using MediatR;
using System.Text.Json.Serialization;

namespace Management.Core.Commands;

public static class UpdateCategory
{
    public class UpdateCategoryRequest : IRequest
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<UpdateCategoryRequest>
    {
        private readonly ICategoryRepository _categoryRepository;

        public Handler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            Category? categoryToUpdate = (await _categoryRepository.GetById(request.Id))!;

            Category? existingCategory = await _categoryRepository.GetByName(request.Name);
            if (existingCategory is not null && existingCategory.Id != request.Id)
            {
                throw new BusinessLogicException($"The category name '{request.Name}' has been used.");
            }

            categoryToUpdate.Name = request.Name;
            await _categoryRepository.Update(categoryToUpdate);
        }
    }

    public class Validator : AbstractValidator<UpdateCategoryRequest>
    {
        public Validator(ICategoryRepository categoryRepository)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    bool categoryExist = await categoryRepository.Exists(id);
                    return categoryExist;
                }).WithMessage("No category with the given ID found.");
        }
    }
}
