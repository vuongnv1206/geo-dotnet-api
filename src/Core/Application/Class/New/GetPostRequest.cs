using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class GetPostRequest : PaginationFilter, IRequest<PaginationResponse<PostDto>>
{
    public Guid ClassId { get; set; }
}

public class GetNewsRequestHandler : IRequestHandler<GetPostRequest, PaginationResponse<PostDto>>
{
    private readonly IReadRepository<Post> _repository;
    private readonly IPostLikeRepository _newReactionRepository;
    private readonly IUserService _userService;
    private readonly IRepository<Classes> _classRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;

    public GetNewsRequestHandler(
        IReadRepository<Post> repository,
        IPostLikeRepository newReactionRepository,
        IUserService userService,
        IRepository<Classes> classRepo,
        ICurrentUser currentUser,
        IStringLocalizer<GetNewsRequestHandler> t)
    {
        _repository = repository;
        _newReactionRepository = newReactionRepository;
        _userService = userService;
        _classRepo = classRepo;
        _currentUser = currentUser;
        _t = t;
    }

    public async Task<PaginationResponse<PostDto>> Handle(GetPostRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        _ = await _classRepo.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassId, userId), cancellationToken)
            ?? throw new NotFoundException(_t["Class {0} Not Found", request.ClassId]);

        var spec = new PostByClassIdSpec(request);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);


        foreach (var posts in data.Data)
        {
            try
            {
                var user = await _userService.GetAsync(posts.CreatedBy.ToString(), cancellationToken);
                if (user != null)
                {
                    posts.Owner = user;
                }

                foreach (var comment in posts.Comments)
                {
                    var userComment = await _userService.GetAsync(comment.CreatedBy.ToString(), cancellationToken);
                    if (userComment != null)
                    {
                        comment.Owner = userComment;
                    }
                }
            }
            catch
            {

            }
        }

        return data;
    }
}