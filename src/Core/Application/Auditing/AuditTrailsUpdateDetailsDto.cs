namespace FSH.WebApi.Application.Auditing;
public class AuditTrailsUpdateDetailsDto
{
    public string Id { get; set; }
    public Dictionary<string, string> OldData { get; set; }
    public Dictionary<string, string> NewData { get; set; }
    public string[] ChangeFields { get; set; }
}
