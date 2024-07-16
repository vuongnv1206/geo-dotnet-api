using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;

namespace FSH.WebApi.Application.Class.Dto;
public class ClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public required string Name { get; set; }
    public required string SchoolYear { get; set; }
    public Guid OwnerId { get; set; }
    public Guid GroupClassId { get; set; }
    public required string GroupClassName { get; set; }
    public int? NumberUserOfClass { get; set; }
    public List<AssignmentDto>? Assignments { get; set; }
    public List<UserStudentDto>? Students { get; set; }
    public List<PaperInListDto>? Papers { get; set; }
    public List<PermissionInClassDto>? Permissions { get; set; }
}
