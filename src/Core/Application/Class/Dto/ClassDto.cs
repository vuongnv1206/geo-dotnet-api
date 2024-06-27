using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using System.Security.Cryptography.X509Certificates;

namespace FSH.WebApi.Application.Class.Dto;
public class ClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; }
    public string SchoolYear { get; set; }
    public Guid OwnerId { get; set; }
    public Guid GroupClassId { get; set; }
    public string GroupClassName { get; set; }
    public int? NumberUserOfClass { get; set; }
    public List<AssignmentDto>? Assignments { get; set; }
    public List<UserStudent>? UserStudents { get; set; }
    public List<PaperInListDto>? Papers{ get; set; }
}
