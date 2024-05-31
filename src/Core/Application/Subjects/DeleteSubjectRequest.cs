using FSH.WebApi.Application.Assignments;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Subjects;

namespace FSH.WebApi.Application.Subjects;
public class DeleteSubjectRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }

    public DeleteSubjectRequest(DefaultIdType id) => Id = id;
}

public class DeleteSubjectRequestHandler : IRequestHandler<DeleteSubjectRequest, DefaultIdType>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Subject> _subjectRepo;
    private readonly IReadRepository<Assignment> _assignmentRepo;
    private readonly IStringLocalizer _t;

    public DeleteSubjectRequestHandler(IRepositoryWithEvents<Subject> subjectRepo, IReadRepository<Assignment> assignmentRepo, IStringLocalizer<DeleteSubjectRequestHandler> localizer) =>
        (_subjectRepo, _assignmentRepo, _t) = (subjectRepo, assignmentRepo, localizer);

    public async Task<DefaultIdType> Handle(DeleteSubjectRequest request, CancellationToken cancellationToken)
    {
        if (await _assignmentRepo.AnyAsync(new AssignmentsBySubjectSpec(request.Id), cancellationToken))
        {
            throw new ConflictException(_t["Subject cannot be deleted as it's being used."]);
        }

        var subject = await _subjectRepo.GetByIdAsync(request.Id, cancellationToken);

        _ = subject ?? throw new NotFoundException(_t["Subject {0} Not Found."]);

        await _subjectRepo.DeleteAsync(subject, cancellationToken);

        return request.Id;
    }
}