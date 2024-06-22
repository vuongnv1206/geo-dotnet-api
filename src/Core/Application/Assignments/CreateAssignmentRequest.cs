using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
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
    //public List<FileUploadRequest>? Attachments { get; set; }

    public List<Guid>? ClassIds { get; set; }
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
        if (request.ClassIds != null)
        {

            foreach (var classId in request.ClassIds)
            {
                var assignmentClass = new AssignmentClass(assignment.Id, classId);
                assignment.AssignmentClasses.Add(assignmentClass);
            }

            await _repository.AddAsync(assignment, cancellationToken);
        }

        return assignment.Id;
    }

    //public async Task<Guid> Handle(CreateAssignmentRequest request, CancellationToken cancellationToken)
    //{
    //    List<string> attachmentPaths = new List<string>();

    //    if (request.Attachments != null && request.Attachments.Any())
    //    {
    //        foreach (var attachment in request.Attachments)
    //        {
    //            string attachmentPath = await _file.UploadAsync<Assignment>(attachment, FileType.Image, cancellationToken);
    //            attachmentPaths.Add(attachmentPath);
    //        }
    //    }

    //    // Convert the list of paths to a single string, or handle it as needed
    //    string combinedAttachmentPaths = string.Join(";", attachmentPaths); // Example of combining paths

    //    var assignment = new Assignment(request.Name, request.StartTime, request.EndTime, combinedAttachmentPaths, request.Content, request.CanViewResult, request.RequireLoginToSubmit, request.SubjectId);

    //    // Add Domain Events to be raised after the commit
    //    assignment.DomainEvents.Add(EntityCreatedEvent.WithEntity(assignment));

    //    await _repository.AddAsync(assignment, cancellationToken);

    //    return assignment.Id;
    //}

}