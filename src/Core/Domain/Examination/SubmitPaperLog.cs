namespace FSH.WebApi.Domain.Examination;
public class SubmitPaperLog : AuditableEntity, IAggregateRoot
{
    public Guid SubmitPaperId { get; set; }
    public virtual SubmitPaper? SubmitPaper { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? PublicIp { get; set; }
    public string? LocalIp { get; set; }
    public string? ProcessLog { get; set; }
    public string? MouseLog { get; set; }
    public string? KeyboardLog { get; set; }
    public string? NetworkLog { get; set; }
    public bool? IsSuspicious { get; set; }

    public SubmitPaperLog()
    {
    }

    public SubmitPaperLog(Guid submitPaperId, string deviceId, string deviceName, string deviceType, string publicIp, string localIp, string processLog, string mouseLog, string keyboardLog, string networkLog, bool isSuspicious)
    {
        SubmitPaperId = submitPaperId;
        DeviceId = deviceId;
        DeviceName = deviceName;
        DeviceType = deviceType;
        PublicIp = publicIp;
        LocalIp = localIp;
        ProcessLog = processLog;
        MouseLog = mouseLog;
        KeyboardLog = keyboardLog;
        NetworkLog = networkLog;
        IsSuspicious = isSuspicious;
    }

    public SubmitPaperLog Update(string? deviceId, string? deviceName, string? deviceType, string? publicIp, string? localIp, string? processLog, string? mouseLog, string? keyboardLog, string? networkLog, bool? isSuspicious)
    {
        if (deviceId is not null && !DeviceId.Equals(deviceId))
        {
            DeviceId = deviceId;
        }

        if (deviceName is not null && !DeviceName.Equals(deviceName))
        {
            DeviceName = deviceName;
        }

        if (deviceType is not null && !DeviceType.Equals(deviceType))
        {
            DeviceType = deviceType;
        }

        if (publicIp is not null && !PublicIp.Equals(publicIp))
        {
            PublicIp = publicIp;
        }

        if (localIp is not null && !LocalIp.Equals(localIp))
        {
            LocalIp = localIp;
        }

        if (processLog is not null && !ProcessLog.Equals(processLog))
        {
            ProcessLog = processLog;
        }

        if (mouseLog is not null && !MouseLog.Equals(mouseLog))
        {
            MouseLog = mouseLog;
        }

        if (keyboardLog is not null && !KeyboardLog.Equals(keyboardLog))
        {
            KeyboardLog = keyboardLog;
        }

        if (networkLog is not null && !NetworkLog.Equals(networkLog))
        {
            NetworkLog = networkLog;
        }

        if (isSuspicious.HasValue && IsSuspicious != isSuspicious)
        {
            IsSuspicious = isSuspicious.Value;
        }

        return this;
    }
}


