namespace FSH.WebApi.Application.Class.Dto;
public class ClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default;
    public string SchoolYear { get; set; }
    public Guid OwnerId { get; set; }
    public Guid GroupClassId { get; set; }
    public string GroupClassName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName{ get; set; }
}
