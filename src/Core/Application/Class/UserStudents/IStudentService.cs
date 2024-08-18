using Microsoft.AspNetCore.Http;

namespace FSH.WebApi.Application.Class.UserStudents;
public interface IStudentService : IScopedService
{
    byte[] GenerateFormatImportStudent();

    Task<List<CreateStudentDto>> GetImportStudents(IFormFile file, Guid classId);
}
