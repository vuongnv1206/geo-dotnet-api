namespace FSH.WebApi.Application.Payment;
public class CancelOrderRequest : IRequest<Unit>
{
    public Guid OrderId { get; set; }
    public CancelOrderRequest(Guid orderId)
    {
        OrderId = orderId;
    }
}

public class CancelOrderRequestHandler : IRequestHandler<CancelOrderRequest>
{
    private readonly IPaymentService _paymentService;

    private readonly ICurrentUser _currentUser;

    public CancelOrderRequestHandler(IPaymentService paymentService, ICurrentUser currentUser)
    {
        _paymentService = paymentService;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(CancelOrderRequest request, CancellationToken cancellationToken)
    {

        await _paymentService.CancelOrder(_currentUser.GetUserId(), request.OrderId);
        return Unit.Value;
    }
}
