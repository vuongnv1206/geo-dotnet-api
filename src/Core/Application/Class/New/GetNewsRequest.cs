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
public class GetNewsRequest : PaginationFilter,IRequest<PaginationResponse<NewsDto>>
{
}
public class GetNewsRequestHandler : IRequestHandler<GetNewsRequest, PaginationResponse<NewsDto>>
{
    private readonly IReadRepository<News> _repository;
    private readonly INewReactionRepository _newReactionRepository;

    public GetNewsRequestHandler(IReadRepository<News> repository, INewReactionRepository newReactionRepository)
    {
        _repository = repository;
        _newReactionRepository = newReactionRepository;
    }

    public async Task<PaginationResponse<NewsDto>> Handle(GetNewsRequest request, CancellationToken cancellationToken)
    {
        var spec = new NewByClassIdSpec(request);
        var data = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
        return data;
    }
}

