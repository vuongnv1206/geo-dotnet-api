using Mapster;
using Microsoft.AspNetCore.Http;

namespace FSH.WebApi.Application.Class.UserStudents;
public class ImportStudentExcelRequest : IRequest<Guid>
{
    public Guid ClassId { get; set; } = default!;
    public IFormFile File { get; set; } = default!;

    public ImportStudentExcelRequest(Guid classId, IFormFile file)
    {
        ClassId = classId;
        File = file;
    }
}

public class ImportStudentExcelRequestHandler : IRequestHandler<ImportStudentExcelRequest, Guid>
{
    private readonly IStudentService _studentService;
    private readonly IMediator _mediator;

    public ImportStudentExcelRequestHandler(IStudentService studentService, IMediator mediator)
    {
        _studentService = studentService;
        _mediator = mediator;
    }

    public async Task<DefaultIdType> Handle(ImportStudentExcelRequest request, CancellationToken cancellationToken)
    {

        var students = await _studentService.GetImportStudents(request.File, request.ClassId);

        var studentRequests = students.Adapt<List<CreateStudentRequest>>();

        foreach(var student in studentRequests)
        {
            await _mediator.Send(student);
        }

        return default(DefaultIdType);
    }
}
