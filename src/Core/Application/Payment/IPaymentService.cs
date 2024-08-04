namespace FSH.WebApi.Application.Payment;
public interface IPaymentService : ITransientService
{
    public Task CheckNewTransactions();
    public Task DeactiveExpiredUser();
    public Task<List<SubscriptionDto>> GetSubcriptions();
    public Task<string> CreateOrder(Guid userId, Guid subscriptionId);
    public Task CancelOrder(Guid userId, Guid orderId);
}
