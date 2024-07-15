namespace FSH.WebApi.Application.Class.GroupClasses.Dto;
public class GroupClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Name { get; set; }
    public List<ClassViewListDto>? Classes { get; set; } = new();
    public Guid CreatedBy { get; set; }
}
