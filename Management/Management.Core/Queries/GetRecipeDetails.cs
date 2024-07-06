using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Exceptions;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using MediatR;

namespace Management.Core.Queries;

public static class GetRecipeDetails
{
    public class GetRecipeDetailsRequest : IRequest<RecipeDto>
    {
        public int Id { get; set; }
    }

    public class Handler : IRequestHandler<GetRecipeDetailsRequest, RecipeDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageRepository _imageRepository;

        public Handler(IUserRepository userRepository, IRecipeRepository recipeRepository, ICategoryRepository categoryRepository, IImageRepository imageRepository)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _categoryRepository = categoryRepository;
            _imageRepository = imageRepository;
        }

        public async Task<RecipeDto> Handle(GetRecipeDetailsRequest request, CancellationToken cancellationToken)
        {
            Recipe entity = await _recipeRepository.GetById(request.Id)
                ?? throw new NotFoundException($"No recipe with the given ID '{request.Id}' found.");

            string? imageUrl = !string.IsNullOrEmpty(entity.ImageFileName) ? _imageRepository.GetImageUrl(entity.ImageFileName!) : null;
            Category? category = entity.CategoryId.HasValue ? await _categoryRepository.GetById(entity.CategoryId.Value) : null;
            UserDto user = await _userRepository.GetUser(entity.UserId);
            return entity.ToDto(imageUrl, category?.Name, user);
        }
    }
}
