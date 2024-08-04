namespace FSH.WebApi.Application.Payment;
public class CreateOrderRequest : IRequest<string>
{
    public Guid SubscriptionId { get; set; }
}

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
    }
}

public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, string>
{

    private readonly IPaymentService _paymentService;
    private readonly ICurrentUser _currentUser;
    public CreateOrderRequestHandler(IPaymentService paymentService, ICurrentUser currentUser)
    {
        _paymentService = paymentService;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        return await _paymentService.CreateOrder(_currentUser.GetUserId(), request.SubscriptionId);
    }
}