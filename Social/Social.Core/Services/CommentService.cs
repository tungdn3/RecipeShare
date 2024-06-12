using MassTransit;
using Shared.IntegrationEvents;
using Social.Core.Dto;
using Social.Core.Entities;
using Social.Core.Exceptions;
using Social.Core.Interfaces;

namespace Social.Core.Services;

public class CommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CommentService(ICommentRepository commentRepository, IUserRepository userRepository, IPublishEndpoint publishEndpoint)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<PageResultDto<CommentDto>> GetByRecipe(int recipeId, int pageNumber, int pageSize)
    {
        PageResultDto<CommentDto> result = await _commentRepository.GetByRecipe(recipeId, pageNumber, pageSize);
        FillUserInfo(result);
        return result;
    }

    public async Task<PageResultDto<CommentDto>> GetReplies(int commentId, int pageNumber, int pageSize)
    {
        PageResultDto<CommentDto> result = await _commentRepository.GetReplies(commentId, pageNumber, pageSize);
        FillUserInfo(result);
        return result;
    }

    public async Task<int> Add(CommentRequest request)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrEmpty(request.Content))
        {
            errors.Add(new ValidationError
            {
                Message = "Content is required.",
                Property = nameof(request.Content),
            });
        }

        if (request.RecipeId == 0)
        {
            errors.Add(new ValidationError
            {
                Message = "RecipeId is required.",
                Property = nameof(request.RecipeId),
            });
        }
        else
        {
            // Check recipe exists???
        }

        Comment? parent = null;
        if (request.ParentId.HasValue)
        {
            parent = await _commentRepository.GetById(request.ParentId.Value);
            if (parent is null)
            {
                errors.Add(new ValidationError
                {
                    Message = "Invalid parent comment.",
                    Property = nameof(request.ParentId),
                });
            }
        }

        if (errors.Count > 0)
        {
            throw new ValidationException("Validation failure", errors);
        }

        var now = DateTime.UtcNow;
        string commentAuthorId = await _userRepository.GetCurrentUserId();
        Comment comment = new Comment
        {
            Content = request.Content,
            RecipeId = request.RecipeId,
            UserId = commentAuthorId,
            Path = parent is not null ? parent.BuildChildrenPath() : string.Empty,
            CreatedAt = now,
            UpdatedAt = null,
            ParentId = request.ParentId
        };

        int id = await _commentRepository.Add(comment);

        await _publishEndpoint.Publish(new CommentAdded
        {
            CommentId = id,
            ParentCommentId = request.ParentId,
            CommentedAt = now,
            RecipeId = request.RecipeId,
            UserId = commentAuthorId
        });

        return id;
    }

    public async Task FillUserInfo(PageResultDto<CommentDto> result)
    {
        string[] userIds = result.Items.Select(x => x.UserId).ToArray();
        List<UserDto> users = await _userRepository.GetUsers(userIds);
        foreach (var item in result.Items)
        {
            var user = users.Find(x => x.Id == item.UserId);
            if (user != null)
            {
                item.UserDisplayName = user.DisplayName;
                item.UserAvatarUrl = user.AvatarUrl;
            }
        }
    }

    public async Task Delete(int id)
    {
        Comment? comment = await _commentRepository.GetById(id);
        if (comment == null)
        {
            return;
        }

        if (comment.UserId != await _userRepository.GetCurrentUserId())
        {
            throw new ForbiddenException("You are not allowed to delete this comment.");
        }

        await _commentRepository.Delete(id);
    }

    public async Task Edit(int id, EditCommentRequest request)
    {
        Comment? comment = await _commentRepository.GetById(id);
        if (comment == null || comment.IsDeleted)
        {
            throw new NotFoundException();
        }

        if (comment.UserId != await _userRepository.GetCurrentUserId())
        {
            throw new ForbiddenException("You are not allowed to delete this comment.");
        }

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.Save(comment);
    }
}
