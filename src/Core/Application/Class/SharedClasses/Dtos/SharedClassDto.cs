using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Application.Examination.Papers;

namespace FSH.WebApi.Application.Class.SharedClasses;
public class SharedClassDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; }
    public string SchoolYear { get; set; }
    public Guid OwnerId { get; set; }
    public Guid GroupClassId { get; set; }
    public string GroupClassName { get; set; }
    public int? NumberUserOfClass { get; set; }
}
