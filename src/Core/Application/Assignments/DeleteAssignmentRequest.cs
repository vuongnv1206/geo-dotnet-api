using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Assignments;

public class DeleteAssignmentRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }

    public DeleteAssignmentRequest(DefaultIdType id) => Id = id;
}

public class DeleteAssignmentRequestHandler : IRequestHandler<DeleteAssignmentRequest, DefaultIdType>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IStringLocalizer _t;

    public DeleteAssignmentRequestHandler(IRepository<Assignment> repository, IStringLocalizer<DeleteAssignmentRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<DefaultIdType> Handle(DeleteAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found."]);

        // Add Domain Events to be raised after the commit
        assignment.DomainEvents.Add(EntityDeletedEvent.WithEntity(assignment));

        await _repository.DeleteAsync(assignment, cancellationToken);

        return request.Id;
    }
}