﻿

namespace FSH.WebApi.Application.TeacherGroup.PermissionClasses;
public class TeacherPermissionInClassDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string PermissionType { get; set; }
}