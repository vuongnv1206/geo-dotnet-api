using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Domain.Class;
public class Classes : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public string SchoolYear { get; private set; }
    public Guid OwnerId { get; private set; }
    public Guid? GroupClassId { get; private set; }
    public virtual GroupClass? GroupClass { get; private set; }
    public virtual List<AssignmentClass>? AssignmentClasses { get; set; } = new();
    public virtual List<AssignmentStudent>? AssignmentStudents { get; set; } = new();
    public virtual List<Student>? Students { get; set; } = new();
    public virtual List<UserClass>? UserClasses { get; set; } = new();
    public virtual List<PaperAccess>? PaperAccesses { get; set; } = new();
    public virtual IEnumerable<TeacherPermissionInClass>? TeacherPermissionInClasses { get; set; }
    public virtual IEnumerable<GroupPermissionInClass>? GroupPermissionInClasses { get; set; }

    public Classes()
    {
    }

    public Classes(string name, string schoolYear, Guid ownerId, Guid? groupClassId)
    {
        Name = name;
        SchoolYear = schoolYear;
        OwnerId = ownerId;
        GroupClassId = groupClassId;
    }

    public Classes Update(string? name, string? schoolYear, Guid? ownerId, Guid? groupClassId)
    {
        if (name is not null && Name?.Equals(name) is not true) Name = name;
        if (schoolYear is not null && SchoolYear?.Equals(schoolYear) is not true) SchoolYear = schoolYear;
        if (ownerId.HasValue && ownerId.Value != Guid.Empty && !OwnerId.Equals(ownerId.Value)) OwnerId = ownerId.Value;
        if (groupClassId.HasValue && groupClassId.Value != Guid.Empty && !GroupClassId.Equals(groupClassId.Value)) GroupClassId = groupClassId.Value;
        return this;
    }

    public void AddUserInClass(UserClass userClass)
    {
        UserClasses.Add(userClass);
    }

    public void RemoveUserInClass(UserClass userClass)
    {
        UserClasses.Remove(userClass);
    }

    public void UpdateGroupClassId(Guid? newGroupClassId)
    {
        GroupClassId = newGroupClassId;
    }

    public void AssignAssignmentToClass(AssignmentClass assignmentClass)
    {
        AssignmentClasses.Add(assignmentClass);
    }

    public bool CanUpdate(Guid userId)
    {
        return CreatedBy == userId;
    }

    public void RemoveAssignmentFromClass(AssignmentClass assignmentClass)
    {
        AssignmentClasses.Remove(assignmentClass);
    }
}
