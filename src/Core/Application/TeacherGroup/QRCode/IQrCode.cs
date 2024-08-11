namespace FSH.WebApi.Application.TeacherGroup.QRCode;
public interface IQrCode : IScopedService
{
    string CreateQrCode(Dictionary<string, string> parameters, string? link);
}