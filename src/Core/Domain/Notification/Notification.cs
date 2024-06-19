using static FSH.WebApi.Shared.Notifications.BasicNotification;

namespace FSH.WebApi.Domain.Notification;
public class Notification : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public LabelType Type { get; set; }
    public string Message { get; set; }
    public string? Url { get; set; }
    public bool IsRead { get; set; }

    public Notification(Guid userId, string title, LabelType type, string message, string? url)
    {
        UserId = userId;
        Title = title;
        Type = type;
        Message = message;
        IsRead = false;
        Url = url;
    }

    public void UpdateIsRead()
    {
        IsRead = !IsRead;
    }

}
