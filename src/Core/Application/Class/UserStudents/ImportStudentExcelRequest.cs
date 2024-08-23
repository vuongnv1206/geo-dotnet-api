using Mapster;
using Microsoft.AspNetCore.Http;

namespace FSH.WebApi.Application.Class.UserStudents;
public class ImportStudentExcelRequest : IRequest<List<FailedStudentRequest>>
{
    public Guid ClassId { get; set; } = default!;
    public IFormFile File { get; set; } = default!;

    public ImportStudentExcelRequest(Guid classId, IFormFile file)
    {
        ClassId = classId;
        File = file;
    }
}

public class ImportStudentExcelRequestHandler : IRequestHandler<ImportStudentExcelRequest, List<FailedStudentRequest>>
{
    private readonly IStudentService _studentService;
    private readonly IMediator _mediator;

    public ImportStudentExcelRequestHandler(IStudentService studentService, IMediator mediator)
    {
        _studentService = studentService;
        _mediator = mediator;
    }

    public async Task<List<FailedStudentRequest>> Handle(ImportStudentExcelRequest request, CancellationToken cancellationToken)
    {

        var students = await _studentService.GetImportStudents(request.File, request.ClassId);

        var studentRequests = students.Adapt<List<CreateStudentRequest>>();
        var failedStudents = new List<FailedStudentRequest>();
        foreach (var student in studentRequests)
        {
            try
            {
                await _mediator.Send(student, cancellationToken);
            }
            catch (Exception ex)
            {
                // Thêm sinh viên bị lỗi vào danh sách cùng với thông tin lỗi
                failedStudents.Add(new FailedStudentRequest
                {
                    StudentRequest = student,
                    ErrorMessage = ex.Message
                });
            }
        }

        return failedStudents;
    }
}

public class FailedStudentRequest
{
    public CreateStudentRequest StudentRequest { get; set; }
    public string ErrorMessage { get; set; }
}