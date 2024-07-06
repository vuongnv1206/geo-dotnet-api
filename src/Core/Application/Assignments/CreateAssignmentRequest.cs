using FSH.WebApi.Application.Common.FileStorage;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Common.Events;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

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
    [AllowedExtensions(".jpg", ".png", ".jpeg", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".zip", ".rar", ".7z", ".mp4", ".avi", ".mkv", ".flv", ".wmv", ".mov", ".webm", ".mp3", ".wav", ".flac", ".ogg", ".wma", ".json", ".xml", ".csv", ".tsv")]
    [MaxFileSize(20 * 1024 * 1024)]
    public IFormFile[]? Attachment { get; set; }
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
        string[] allowedExtensions = new[] { ".jpg", ".png", ".jpeg", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".zip", ".rar", ".7z", ".mp4", ".avi", ".mkv", ".flv", ".wmv", ".mov", ".webm", ".mp3", ".wav", ".flac", ".ogg", ".wma", ".json", ".xml", ".csv", ".tsv" };
        string allowedExtensionsMessage = string.Join(", ", allowedExtensions);

        foreach (var file in request.Attachment)
        {
            string extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                throw new BadRequestException($"Only {allowedExtensionsMessage} files are allowed.");
            }
        }

        string[] listFilePaths = await _file.SaveFilesAsync(request.Attachment, cancellationToken);

        string attachmentPath = JsonSerializer.Serialize(listFilePaths);

        var assignment = new Assignment(request.Name.Trim(), request.StartTime, request.EndTime, attachmentPath, request.Content, request.CanViewResult, request.RequireLoginToSubmit, request.SubjectId);
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
}