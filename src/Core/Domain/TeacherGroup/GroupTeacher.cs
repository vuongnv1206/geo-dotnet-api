namespace FSH.WebApi.Domain.TeacherGroup;
public class GroupTeacher : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; } = null!;
    public string? JoinLink { get; set; }
    public string? QrCode { get; set; }
    public virtual List<TeacherInGroup> TeacherInGroups { get; set; } = new();
    public virtual List<GroupPermissionInClass> GroupPermissionInClasses { get; set; } = new();
    public virtual List<JoinGroupTeacherRequest> JoinGroupRequests { get; set; } = new();

    public GroupTeacher(string name)
    {
        Name = name;
    }

    public GroupTeacher UpdateJoinGroup(string joinLink, string qrCode)
    {
        JoinLink = joinLink;
        QrCode = qrCode;
        return this;
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

    public void RemoveTeacherInGroup(TeacherInGroup teacher)
    {
        TeacherInGroups.Remove(teacher);
    }

    public bool CanUpdate(Guid userId)
    {
        return CreatedBy == userId;
    }

    public void AddPermission(GroupPermissionInClass permission)
    {
        if (permission is not null)
        {
            GroupPermissionInClasses.Add(permission);
        }
    }

    public void RemovePermission(GroupPermissionInClass permission)
    {
        if (permission is not null)
        {
            GroupPermissionInClasses.Remove(permission);
        }
    }

    public void AddRequestJoinGroup(JoinGroupTeacherRequest request)
    {
        if (request is not null)
        {
            JoinGroupRequests.Add(request);
        }
    }
}
