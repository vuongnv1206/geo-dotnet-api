using FSH.WebApi.Domain.Payment;

namespace FSH.WebApi.Application.Payment;
public class TransactionAPIResponse
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new();
}

public class TransactionDto
{
    public int TransactionID { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string TransactionDate { get; set; }
    public string Type { get; set; }
}
