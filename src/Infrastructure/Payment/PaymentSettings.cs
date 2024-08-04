namespace FSH.WebApi.Infrastructure.Payment;
public class PaymentSettings
{
    public string? TransactionsURL { get; set; }
    public string? SyncJobURL { get; set; }
    public string? CheckTransCron { get; set; }
    public string? DisableSubCron { get; set; }
}
