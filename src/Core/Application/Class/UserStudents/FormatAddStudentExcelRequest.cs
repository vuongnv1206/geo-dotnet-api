namespace FSH.WebApi.Application.Class.UserStudents;
public class FormatAddStudentExcelRequest : IRequest<byte[]>
{
}

public class FormatAddStudentExcelRequestHandler : IRequestHandler<FormatAddStudentExcelRequest, byte[]>
{
    private readonly IStudentService _studentService;

    public FormatAddStudentExcelRequestHandler(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<byte[]> Handle(FormatAddStudentExcelRequest request, CancellationToken cancellationToken)
    {
        var resutl = _studentService.GenerateFormatImportStudent();
        return resutl;
    }
}
