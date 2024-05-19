

namespace FSH.WebApi.Domain.TeacherGroup;
public class GroupTeacher : AuditableEntity,IAggregateRoot
{
    public string Name { get; set; } = null!;
    public virtual List<TeacherInGroup>? TeacherInGroups { get; set; } = new();
    public virtual IEnumerable<GroupPermissionInClass> GroupPermissionInClasses { get; set; }

    public GroupTeacher(string name)
    {
        Name = name;
    }

    public GroupTeacher Update(string? name)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        return this;
    }

    public void AddTeacherIntoGroup(TeacherInGroup teacher)
    {
        TeacherInGroups.Add(teacher);
    }

    public bool CanUpdate(Guid userId)
    {
        return CreatedBy == userId;
    }
}
