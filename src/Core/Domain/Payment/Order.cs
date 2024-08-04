namespace FSH.WebApi.Domain.Payment;
public class Order : AuditableEntity, IAggregateRoot
{
    public string OrderNo { get; set; }
    public Guid UserId { get; set; }
    public Guid SupscriptionId { get; set; }
    public decimal Total { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public OrderStatus Status { get; set; }
    public bool IsExpired { get; set; }
    public Subscription Subscription { get; set; }
}

public enum OrderStatus
{
    PENDING,
    COMPLETED,
    CANCELLED,
    AUTOCANCELLED,
}
