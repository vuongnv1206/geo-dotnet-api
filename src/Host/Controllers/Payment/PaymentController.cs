using FSH.WebApi.Application.Payment;

namespace FSH.WebApi.Host.Controllers.Payment;
public class PaymentController : VersionedApiController
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("check-new-transactions")]
    [AllowAnonymous]
    [OpenApiOperation("Create a job to check new transactions.", "")]
    public Task CreateJobAsync()
    {
        return _paymentService.CheckNewTransactions();
    }

    [HttpGet("deactive-expired-user")]
    [AllowAnonymous]
    [OpenApiOperation("Create a job to deactive expired user.", "")]
    public Task DeactiveExpiredUserAsync()
    {
        return _paymentService.DeactiveExpiredUser();
    }

    [HttpGet("subcriptions")]
    [AllowAnonymous]
    [OpenApiOperation("Get all subcriptions.", "")]
    public async Task<List<SubcriptionDto>> GetSubcriptionsAsync()
    {
        return await _paymentService.GetSubcriptions();
    }
}

