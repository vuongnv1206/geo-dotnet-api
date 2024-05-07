using FSH.WebApi.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Subjects;
public class SearchSubjectsRequest : PaginationFilter, IRequest<PaginationResponse<SubjectDto>>
{
}

public class SubjectsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Subject, SubjectDto>
{
    public SubjectsBySearchRequestSpec(SearchSubjectsRequest request)
        : base(request) =>
        Query.OrderBy(c => c.Name, !request.HasOrderBy());
}

public class SearchSubjectsRequestHandler : IRequestHandler<SearchSubjectsRequest, PaginationResponse<SubjectDto>>
{
    private readonly IReadRepository<Subject> _repository;

    public SearchSubjectsRequestHandler(IReadRepository<Subject> repository) => _repository = repository;

    public async Task<PaginationResponse<SubjectDto>> Handle(SearchSubjectsRequest request, CancellationToken cancellationToken)
    {
        var spec = new SubjectsBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}