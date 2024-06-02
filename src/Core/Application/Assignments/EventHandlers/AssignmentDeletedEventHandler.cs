using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Common.Events;
namespace FSH.WebApi.Application.Assignments.EventHandlers;

public class AssignmentDeletedEventHandler : EventNotificationHandler<EntityDeletedEvent<Assignment>>
{
    private readonly ILogger<AssignmentDeletedEventHandler> _logger;

    public AssignmentDeletedEventHandler(ILogger<AssignmentDeletedEventHandler> logger) => _logger = logger;

    public override Task Handle(EntityDeletedEvent<Assignment> @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", @event.GetType().Name);
        return Task.CompletedTask;
    }
}