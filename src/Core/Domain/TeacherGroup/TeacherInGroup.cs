namespace FSH.WebApi.Domain.TeacherGroup;
public class TeacherInGroup
{
    public Guid TeacherId { get; set; }
    public Guid GroupTeacherId { get; set; }
    public virtual GroupTeacher GroupTeacher { get; set; }
}
