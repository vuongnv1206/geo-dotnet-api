using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class GetNewsRequest : PaginationFilter, IRequest<PaginationResponse<NewsDto>>
{
    public Guid? ClassesId { get; set; }
    public class GetNewsRequestHandler : IRequestHandler<GetNewsRequest, PaginationResponse<NewsDto>>
    {
        private readonly IReadRepository<News> _repository;
        public GetNewsRequestHandler(IReadRepository<News> repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResponse<NewsDto>> Handle(GetNewsRequest request, CancellationToken cancellationToken)
        {
            var spec = new NewsBySearchRequestWithClass(request);
            return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
        }
    }
}

