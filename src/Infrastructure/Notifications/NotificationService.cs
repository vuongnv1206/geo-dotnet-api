using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Shared.Notifications;

namespace FSH.WebApi.Infrastructure.Notifications;
public class NotificationService : INotificationService
{
    public Task SendNotification(INotificationMessage notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SendRemindNotification(DateTime dateTime, INotificationMessage notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SendUserNotification(string userId, INotificationMessage notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SendUserRemindNotification(DateTime dateTime, string userId, INotificationMessage notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SendUsersNotification(List<string> userIds, INotificationMessage notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SendUsersRemindNotification(DateTime dateTime, List<string> userIds, INotificationMessage notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
