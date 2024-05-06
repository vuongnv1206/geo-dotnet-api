

namespace FSH.WebApi.Domain.TeacherGroup;
public class GroupTeacher : AuditableEntity,IAggregateRoot
{
    public string Name { get; set; } = null!;
    public virtual IEnumerable<TeacherInGroup>? TeacherInGroups { get; set; }

    public GroupTeacher(string name)
    {
        Name = name;
    }

    public GroupTeacher Update(string? name)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        return this;
    }
}
