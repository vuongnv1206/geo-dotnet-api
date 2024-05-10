using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Assignments;
public class CreateAssignmentRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public Guid SubjectId { get; set; }
    public FileUploadRequest? Attachment { get; set; }
}

public class CreateAssignmentRequestHandler : IRequestHandler<CreateAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IFileStorageService _file;

    public CreateAssignmentRequestHandler(IRepository<Assignment> repository, IFileStorageService file) =>
        (_repository, _file) = (repository, file);

    public async Task<Guid> Handle(CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        string productAttachmentPath = await _file.UploadAsync<Assignment>(request.Attachment, FileType.Image, cancellationToken);

        var assignment = new Assignment(request.Name, request.StartTime, request.EndTime, productAttachmentPath, request.Content, request.CanViewResult, request.RequireLoginToSubmit, request.SubjectId);

        // Add Domain Events to be raised after the commit
        assignment.DomainEvents.Add(EntityCreatedEvent.WithEntity(assignment));

        await _repository.AddAsync(assignment, cancellationToken);

        return assignment.Id;
    }
}
