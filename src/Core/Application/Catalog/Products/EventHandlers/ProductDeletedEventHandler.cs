using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Catalog.Products.EventHandlers;

public class AssignmentDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Product>>
{
    private readonly ILogger<AssignmentDeletedEventHandler> _logger;

    public AssignmentDeletedEventHandler(ILogger<AssignmentDeletedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityDeletedEvent<Product> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}