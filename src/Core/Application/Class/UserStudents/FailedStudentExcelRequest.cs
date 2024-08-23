namespace FSH.WebApi.Application.Class.UserStudents;
public class FailedStudentExcelRequest : IRequest<byte[]>
{
    public List<FailedStudentRequest> FailedStudents { get; set; }
}

public class FailedStudentExcelRequestHandler : IRequestHandler<FailedStudentExcelRequest, byte[]>
{
    private readonly IStudentService _studentService;

    public FailedStudentExcelRequestHandler(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<byte[]> Handle(FailedStudentExcelRequest request, CancellationToken cancellationToken)
    {
        var resutl = _studentService.GenerateImportStudentFailed(request.FailedStudents);
        return resutl;
    }
}

