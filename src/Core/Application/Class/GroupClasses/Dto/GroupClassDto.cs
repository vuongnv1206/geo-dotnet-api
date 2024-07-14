using FSH.WebApi.Application.Class.Dto;

namespace FSH.WebApi.Application.Class.GroupClasses.Dto;
public class GroupClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Name { get; set; }
    public List<ClassDto>? Classes { get; set; } = new();
    public Guid CreatedBy { get; set; }
}
