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


    public GetNewsRequestHandler(IReadRepository<Post> repository, IPostLikeRepository newReactionRepository, IUserService userService)
    {
        _repository = repository;
        _newReactionRepository = newReactionRepository;
        _userService = userService;
    }

    public async Task<PaginationResponse<PostDto>> Handle(GetPostRequest request, CancellationToken cancellationToken)
    {
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