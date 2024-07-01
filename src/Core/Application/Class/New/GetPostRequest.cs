using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class GetPostRequest : PaginationFilter, IRequest<PaginationResponse<PostDto>>
{
}

public class GetNewsRequestHandler : IRequestHandler<GetPostRequest, PaginationResponse<PostDto>>
{
    private readonly IReadRepository<Post> _repository;
    private readonly IPostLikeRepository _newReactionRepository;

    public GetNewsRequestHandler(IReadRepository<Post> repository, IPostLikeRepository newReactionRepository)
    {
        _repository = repository;
        _newReactionRepository = newReactionRepository;
    }

    public async Task<PaginationResponse<PostDto>> Handle(GetPostRequest request, CancellationToken cancellationToken)
    {
        var spec = new PostByClassIdSpec(request);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
        return data;
    }
}

