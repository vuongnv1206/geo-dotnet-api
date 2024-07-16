namespace FSH.WebApi.Application.Class;
public class ClassViewListDto : IDto
{
    public DefaultIdType Id { get; set; }
    public required string Name { get; set; }
    public required string SchoolYear { get; set; }
    public Guid OwnerId { get; set; }
    public Guid GroupClassId { get; set; }
    public required string GroupClassName { get; set; }
    public int? NumberUserOfClass { get; set; }
}
