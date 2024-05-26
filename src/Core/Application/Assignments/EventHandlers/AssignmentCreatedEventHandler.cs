using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Assignments.EventHandlers;

public class AssignmentCreatedEventHandler : EventNotificationHandler<EntityCreatedEvent<Assignment>>
{
    private readonly ILogger<AssignmentCreatedEventHandler> _logger;

    public AssignmentCreatedEventHandler(ILogger<AssignmentCreatedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityCreatedEvent<Assignment> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}