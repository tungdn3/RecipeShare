using Management.Core.Dtos;
using Management.Core.Entities;
using Management.Core.Extensions;
using Management.Core.Interfaces;
using MediatR;

namespace Management.Core.Queries;

public static class GetRecipesByCurrentUser
{
    public class GetRecipesByCurrentUserRequest : IRequest<PageResultDto<RecipeDto>>
    {
        public string? Title { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    public class Handler : IRequestHandler<GetRecipesByCurrentUserRequest, PageResultDto<RecipeDto>>
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

        public async Task<PageResultDto<RecipeDto>> Handle(GetRecipesByCurrentUserRequest request, CancellationToken cancellationToken)
        {
            string currentUserId = await _userRepository.GetCurrentUserId();
            PageResultDto<Recipe> entityResult = await _recipeRepository.GetByUserId(currentUserId, request.Title, request.PageNumber, request.PageSize);

            List<string> authorIds = entityResult.Items.Select(x => x.UserId).ToList();
            List<UserDto> users = await _userRepository.GetUsers(authorIds);

            List<int> categoryIds = entityResult.Items.Where(x => x.CategoryId.HasValue).Select(x => (int)x.CategoryId!).Distinct().ToList();
            List<Category> categories = await _categoryRepository.GetByIds(categoryIds);

            List<RecipeDto> dtos = entityResult.Items.Select(item =>
            {
                string? imageUrl = !string.IsNullOrEmpty(item.ImageFileName) ? _imageRepository.GetImageUrl(item.ImageFileName!) : null;
                Category? category = categories.Find(x => x.Id == item.CategoryId);
                UserDto? user = users.Find(u => u.Id == item.UserId);
                return item.ToDto(imageUrl, category?.Name, user);
            }).ToList();

            return new PageResultDto<RecipeDto>
            {
                Items = dtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = entityResult.TotalCount,
            };
        }
    }
}
