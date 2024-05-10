using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Assignments.EventHandlers;


public class AssignmentUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Product>>
{
    private readonly ILogger<AssignmentUpdatedEventHandler> _logger;

    public AssignmentUpdatedEventHandler(ILogger<AssignmentUpdatedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityUpdatedEvent<Product> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}