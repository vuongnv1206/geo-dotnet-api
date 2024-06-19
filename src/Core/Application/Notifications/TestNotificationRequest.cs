namespace FSH.WebApi.Application.Notifications;
public class TestNotificationRequest : IRequest<string>
{
    public string[] userIds { get; set; }
    public string? Message { get; set; }
}

public class TestNotificationHandler : IRequestHandler<TestNotificationRequest, string>
{
    private readonly INotificationSender _notifications;

    public TestNotificationHandler(INotificationSender notifications)
    {
        _notifications = notifications;
    }

    public async Task<string> Handle(TestNotificationRequest request, CancellationToken cancellationToken)
    {
        var notification = new BasicNotification
        {
            Message = request.Message,
            Label = BasicNotification.LabelType.Information
        };

        await _notifications.SendToUsersAsync(notification, request.userIds, cancellationToken);

        return "Notification sent";
    }
}