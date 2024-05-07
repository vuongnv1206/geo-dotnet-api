using FSH.WebApi.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Subjects;
public class GetSubjectRequest : IRequest<SubjectDto>
{
    public Guid Id { get; set; }

    public GetSubjectRequest(Guid id) => Id = id;
}

public class SubjectByIdSpec : Specification<Subject, SubjectDto>, ISingleResultSpecification
{
    public SubjectByIdSpec(Guid id) =>
        Query.Where(p => p.Id == id);
}

public class GetSubjectRequestHandler : IRequestHandler<GetSubjectRequest, SubjectDto>
{
    private readonly IRepository<Subject> _repository;
    private readonly IStringLocalizer _t;

    public GetSubjectRequestHandler(IRepository<Subject> repository, IStringLocalizer<GetSubjectRequestHandler> localizer) => (_repository, _t) = (repository, localizer);

    public async Task<SubjectDto> Handle(GetSubjectRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            (ISpecification<Subject, SubjectDto>)new SubjectByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(_t["Subject {0} Not Found.", request.Id]);
}