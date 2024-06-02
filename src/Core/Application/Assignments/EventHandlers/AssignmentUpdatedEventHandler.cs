using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Common.Events;

namespace FSH.WebApi.Application.Assignments.EventHandlers;

public class AssignmentUpdatedEventHandler : EventNotificationHandler<EntityUpdatedEvent<Assignment>>
{
    private readonly ILogger<AssignmentUpdatedEventHandler> _logger;

    public AssignmentUpdatedEventHandler(ILogger<AssignmentUpdatedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityUpdatedEvent<Assignment> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}