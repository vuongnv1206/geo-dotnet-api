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
    public string? Attachment { get; set; }
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

        //string attachmentPath = JsonSerializer.Serialize(request.Attachment);

        var assignment = new Assignment(request.Name.Trim(), request.StartTime, request.EndTime, request.Attachment, request.Content, request.CanViewResult, request.RequireLoginToSubmit, request.SubjectId);
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