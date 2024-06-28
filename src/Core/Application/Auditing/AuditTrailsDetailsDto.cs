namespace FSH.WebApi.Application.Auditing;
public class AuditTrailsDetailsDto<T>
{
    public T OldData { get; set; }
    public T NewData { get; set; }
    public string[] ChangeFields { get; set; }
}
