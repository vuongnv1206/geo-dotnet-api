
using System.ComponentModel.DataAnnotations;


namespace FSH.WebApi.Domain.TeacherGroup;
public class TeacherTeam : AuditableEntity, IAggregateRoot
{
    public Guid? TeacherId { get; set; }
    public string TeacherName { get; set; } = null!;
    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? Phone { get; set; }

    public virtual IEnumerable<TeacherPermissionInClass> TeacherPermissionInClasses { get; set; }

    public TeacherTeam Update(string? teacherName, string? teacherEmail, string? teacherPhone)
    {
        if (teacherName is not null && TeacherName?.Equals(teacherName) is not true) TeacherName = teacherName;
        if (teacherEmail is not null && Email?.Equals(teacherEmail) is not true) Email = teacherEmail;
        if (teacherPhone is not null && Phone?.Equals(teacherPhone) is not true) Phone = teacherPhone;

        return this;
    }

    public TeacherTeam UpdateRegistrationStatus(Guid? teacherId)
    {
        if (teacherId is not null && (TeacherId == teacherId) is not true) TeacherId = teacherId;
        return this;
    }
}
