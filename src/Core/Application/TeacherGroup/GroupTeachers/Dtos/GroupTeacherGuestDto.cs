namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class GroupTeacherGuestDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? QrCode { get; set; }
    public string? JoinLink { get; set; }
    public string? AdminGroup { get; set; }
    public Guid CreatedBy { get; set; }
}
