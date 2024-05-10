using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Assignments.EventHandlers;

public class AssignmentCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Product>>
{
    private readonly ILogger<AssignmentCreatedEventHandler> _logger;

    public AssignmentCreatedEventHandler(ILogger<AssignmentCreatedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityCreatedEvent<Product> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}