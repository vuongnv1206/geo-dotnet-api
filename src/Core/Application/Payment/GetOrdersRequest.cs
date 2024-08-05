using FSH.WebApi.Application.Common.Specification;
using FSH.WebApi.Domain.Payment;

namespace FSH.WebApi.Application.Payment;
public class GetOrdersRequest : PaginationFilter, IRequest<PaginationResponse<OrderDto>>
{
    public Guid? SubscriptionId { get; set; }
    public OrderStatus? Status { get; set; }
    public bool? IsExpired { get; set; }
}

public class GetOrdersRequestSpec : EntitiesByPaginationFilterSpec<Order, OrderDto>
{
    public GetOrdersRequestSpec(GetOrdersRequest request, Guid userId)
        : base(request)
    {
        Query
            .Include(s => s.Subscription)
            .Where(p => p.UserId.Equals(userId) &&
                        (!request.SubscriptionId.HasValue || p.SubscriptionId == request.SubscriptionId) &&
                        (!request.Status.HasValue || p.Status == request.Status) &&
                        (!request.IsExpired.HasValue || p.IsExpired == request.IsExpired));
    }
}

public class GetOrdersRequestHandler : IRequestHandler<GetOrdersRequest, PaginationResponse<OrderDto>>
{
    private readonly IReadRepository<Order> _repository;
    private readonly ICurrentUser _currentUser;

    public GetOrdersRequestHandler(IReadRepository<Order> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PaginationResponse<OrderDto>> Handle(GetOrdersRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var spec = new GetOrdersRequestSpec(request, currentUserId);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}

public class OrderDto : IDto
{
    public string OrderNo { get; set; }
    public decimal Total { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public OrderStatus Status { get; set; }
    public bool IsExpired { get; set; }
    public SubscriptionDto Subscription { get; set; }
    public DateTime CreatedOn { get; set; }
}
