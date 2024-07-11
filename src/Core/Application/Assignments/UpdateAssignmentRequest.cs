using FSH.WebApi.Application.Assignments.AssignmentClasses;
using FSH.WebApi.Application.Assignments.AssignmentStudent;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
namespace FSH.WebApi.Application.Assignments;
public class UpdateAssignmentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Content { get; set; }
    public bool CanViewResult { get; set; }
    public bool RequireLoginToSubmit { get; set; }
    public DefaultIdType? SubjectId { get; set; }
    public bool DeleteCurrentAttachment { get; set; } = false;
    public string? Attachment { get; set; }
    public List<Guid>? ClassIds { get; set; }
    public List<Guid>? StudentIds { get; set; }

}

public class UpdateAssignmentRequestHandler : IRequestHandler<UpdateAssignmentRequest, Guid>
{
    private readonly IRepository<Assignment> _repository;
    private readonly IStringLocalizer<UpdateAssignmentRequestHandler> _t;
    private readonly IFileStorageService _file;
    private readonly IMediator _mediator;

    public UpdateAssignmentRequestHandler(
        IRepository<Assignment> repository,
        IStringLocalizer<UpdateAssignmentRequestHandler> t,
        IFileStorageService file,
        IMediator mediator)
        => (_repository, _t, _file, _mediator)
            = (repository, t, file, mediator);

    public async Task<Guid> Handle(UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.Id), cancellationToken);

        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not Found.", request.Id]);

        var updatedAssignment = assignment.Update(
            request.Name,
            request.StartTime,
            request.EndTime,
            request.Attachment,
            request.Content,
            request.CanViewResult,
            request.RequireLoginToSubmit,
            request.SubjectId);

        if (request.ClassIds != null)
        {
            // Lấy danh sách các lớp hiện tại được giao bài tập
            var currentClassIds = updatedAssignment.AssignmentClasses.Select(ac => ac.ClassesId).ToList();
            // Lấy danh sách các lớp cần thêm mới
            var newClassIds = request.ClassIds.Except(currentClassIds).ToList();
            // Lấy danh sách các lớp cần xóa
            var removedClassIds = currentClassIds.Except(request.ClassIds).ToList();
            // Thêm các lớp mới vào AssignmentClasses
            if (newClassIds.Any())
            {
                var assignRequest = new AssignAssignmentToClassRequest(request.Id, newClassIds);
                await _mediator.Send(assignRequest, cancellationToken);
            }
            // Xóa các lớp không còn trong danh sách
            if (removedClassIds.Any())
            {
                foreach (var classId in removedClassIds)
                {
                    var removeRequest = new RemoveAssignmentFromClassRequest(request.Id, classId);
                    await _mediator.Send(removeRequest, cancellationToken);
                }
            }

        }
        else
        {
            // Nếu ClassIds là null, xóa tất cả các lớp hiện tại
            var currentClassIds = updatedAssignment.AssignmentClasses.Select(ac => ac.ClassesId).ToList();
            foreach (var classId in currentClassIds)
            {
                var removeRequest = new RemoveAssignmentFromClassRequest(request.Id, classId);
                await _mediator.Send(removeRequest, cancellationToken);
            }
        }

        if (request.StudentIds != null)
        {
            // Lấy danh sách các học sinh hiện tại được giao bài tập
            var currentStudentIds = updatedAssignment.AssignmentStudents.Select(a => a.StudentId).ToList();
            // Lấy danh sách các học sinh cần thêm mới
            var newStudentIds = request.StudentIds.Except(currentStudentIds).ToList();
            // Lấy danh sách các học sinh cần xóa
            var removedStudentIds = currentStudentIds.Except(request.StudentIds).ToList();

            // Thêm các học sinh mới vào AssignmentStudents
            if (newStudentIds.Any())
            {
                var assignRequest = new AssignAssignmentToStudentsRequest
                {
                    AssignmentId = request.Id,
                    StudentIds = newStudentIds
                };
                await _mediator.Send(assignRequest, cancellationToken);
            }

            // Xóa các học sinh không còn trong danh sách
            if (removedStudentIds.Any())
            {
                foreach (var studentId in removedStudentIds)
                {
                    var removeRequest = new RemoveAssignmentOfStudentRequest
                    {
                        AssignmentId = request.Id,
                        StudentId = studentId
                    };
                    await _mediator.Send(removeRequest, cancellationToken);
                }
            }
        }
        else
        {
            // Nếu StudentIds là null, xóa tất cả các học sinh hiện tại
            var currentStudentIds = updatedAssignment.AssignmentStudents.Select(a => a.StudentId).ToList();
            foreach (var studentId in currentStudentIds)
            {
                var removeRequest = new RemoveAssignmentOfStudentRequest
                {
                    AssignmentId = request.Id,
                    StudentId = studentId
                };
                await _mediator.Send(removeRequest, cancellationToken);
            }
        }

        await _repository.UpdateAsync(updatedAssignment, cancellationToken);

        return request.Id;
    }
}