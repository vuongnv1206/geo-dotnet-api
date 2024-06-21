﻿namespace FSH.WebApi.Shared.Notifications;

public class BasicNotification : INotificationMessage
{
    public enum LabelType
    {
        Information,
        Success,
        Warning,
        Error,
        Reminder
    }

    public string? Message { get; set; }
    public LabelType Label { get; set; }
}