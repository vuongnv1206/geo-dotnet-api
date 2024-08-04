namespace FSH.WebApi.Application.Payment;
public interface IPaymentService : ITransientService
{
    public Task CheckNewTransactions();
    public Task DeactiveExpiredUser();
    public Task<List<SubcriptionDto>> GetSubcriptions();
}
