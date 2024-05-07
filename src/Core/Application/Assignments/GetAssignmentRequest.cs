using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;

public class GetAssignmentRequest : IRequest<AssignmentDetailsDto>
{
    public Guid Id { get; set; }

    public GetAssignmentRequest(Guid id) => Id = id;
}

public class GetAssignmentRequestHandler : IRequestHandler<GetAssignmentRequest, AssignmentDetailsDto>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IStringLocalizer _t;

    public GetAssignmentRequestHandler(IRepository<Assignment> repository, IStringLocalizer<GetAssignmentRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<AssignmentDetailsDto> Handle(GetAssignmentRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            (ISpecification<Assignment, AssignmentDetailsDto>)new AssignmentByIdWithSubjectSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.Id]);
}