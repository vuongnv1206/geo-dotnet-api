namespace FSH.WebApi.Application.Class;
public class ClassViewListDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; }
    public string SchoolYear { get; set; }
    public Guid OwnerId { get; set; }
    public Guid GroupClassId { get; set; }
    public string GroupClassName { get; set; }
    public int? NumberUserOfClass { get; set; }
}
