namespace FSH.WebApi.Application.Notifications;
public interface INotificationService : ITransientService
{
    Task SendNotification(INotificationMessage notification, CancellationToken cancellationToken);
    Task SendUsersNotification(List<string> userIds, INotificationMessage notification, CancellationToken cancellationToken);
    Task SendUserNotification(string userId, INotificationMessage notification, CancellationToken cancellationToken);
    Task SendRemindNotification(DateTime dateTime, INotificationMessage notification, CancellationToken cancellationToken);
    Task SendUsersRemindNotification(DateTime dateTime, List<string> userIds, INotificationMessage notification, CancellationToken cancellationToken);
    Task SendUserRemindNotification(DateTime dateTime, string userId, INotificationMessage notification, CancellationToken cancellationToken);
}
