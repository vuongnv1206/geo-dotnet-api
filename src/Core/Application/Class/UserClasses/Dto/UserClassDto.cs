using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserClasses.Dto;
public class UserClassDto
{
    public Guid ClassesId { get; set; }
    public Guid StudentId { get; set; }
    public required List<Student> UserStudents { get; set; }
}
