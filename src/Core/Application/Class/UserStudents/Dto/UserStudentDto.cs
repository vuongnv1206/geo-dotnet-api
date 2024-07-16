namespace FSH.WebApi.Application.Class.UserStudents.Dto;
public class UserStudentDto : IDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentCode { get; set; }
    public bool? Gender { get; set; }
}
