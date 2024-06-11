﻿using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Domain.Class;
public class Classes : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public string SchoolYear { get; private set; }
    public Guid OwnerId { get; private set; }
    public Guid? GroupClassId { get; private set; }
    public virtual GroupClass GroupClass { get; private set; }

    public virtual List<AssignmentClass> AssignmentClasses { get; set; } = new();
    public virtual List<UserClass> UserClasses { get; set; } = new();

    public Classes(string? name, string? schoolYear, Guid ownerId, Guid? groupClassId)
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

    public void UpdateGroupClassId(Guid? newGroupClassId)
    {
        GroupClassId = newGroupClassId;
    }

    public void AssignAssignmentToClass(AssignmentClass assignmentClass)
    {
        AssignmentClasses.Add(assignmentClass);
    }

    public void RemoveAssignmentFromClass(AssignmentClass assignmentClass)
    {
        AssignmentClasses.Remove(assignmentClass);
    }

    public bool CanUpdate(Guid userId)
    {
        return CreatedBy == userId;
    }

}
