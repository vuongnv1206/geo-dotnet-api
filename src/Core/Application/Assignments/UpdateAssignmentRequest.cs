using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Common.Events;
namespace FSH.WebApi.Application.Assignments;
public class UpdateAssignmentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? AttachmentPath { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public DefaultIdType? SubjectId { get; set; }
    public bool DeleteCurrentAttachment { get; set; } = false;
    public FileUploadRequest? Attachment { get; set; }
    public List<Guid>? ClassIds { get; set; }

}

public class UpdateAssignmentRequestHandler : IRequestHandler<UpdateAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IStringLocalizer _t;
    private readonly IFileStorageService _file;

    public UpdateAssignmentRequestHandler(IRepository<Assignment> repository, IStringLocalizer<UpdateAssignmentRequestHandler> localizer, IFileStorageService file) =>
        (_repository, _t, _file) = (repository, localizer, file);

    public async Task<Guid> Handle(UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.Id), cancellationToken);

        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.Id]);

        // Remove old image if flag is set
        if (request.DeleteCurrentAttachment)
        {
            string? currentAssignmentAttachmentPath = assignment.AttachmentPath;
            if (!string.IsNullOrEmpty(currentAssignmentAttachmentPath))
            {
                string root = Directory.GetCurrentDirectory();
                _file.Remove(Path.Combine(root, currentAssignmentAttachmentPath));
            }

            assignment = assignment.ClearAttachmentPath();
        }

        string? assignmentAttachmentPath = request.Attachment is not null
            ? await _file.UploadAsync<Assignment>(request.Attachment, FileType.Image, cancellationToken)
            : null;

        var updatedAssignment = assignment.Update(
            request.Name.Trim(),
            request.StartTime,
            request.EndTime,
            assignmentAttachmentPath,
            request.Content,
            request.CanViewResult,
            request.RequireLoginToSubmit,
            request.SubjectId);

        if (request.ClassIds != null)
        {
            updatedAssignment.AssignmentClasses.Clear();
            updatedAssignment.UpdateAssignmentFromClass(request.ClassIds.Select(x => new AssignmentClass(updatedAssignment.Id, x)).ToList());

        }

        await _repository.UpdateAsync(updatedAssignment, cancellationToken);

        return request.Id;
    }
}