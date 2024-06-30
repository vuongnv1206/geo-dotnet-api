namespace FSH.WebApi.Application.Auditing;
public class AuditTrailsDto
{
    public string Id { get; set; }
    public AuthorDto Author { get; set; }
    public string Action { get; set; }
    public string Resource { get; set; }
    public string ResourceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
