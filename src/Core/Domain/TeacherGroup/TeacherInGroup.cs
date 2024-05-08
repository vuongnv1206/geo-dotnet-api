using System.ComponentModel;

namespace FSH.WebApi.Domain.TeacherGroup;
public class TeacherInGroup
{
    [Description("TeacherTeamId")]
    public Guid TeacherTeamId { get; set; }
    public Guid GroupTeacherId { get; set; }
    public virtual GroupTeacher GroupTeacher { get; set; }
}
