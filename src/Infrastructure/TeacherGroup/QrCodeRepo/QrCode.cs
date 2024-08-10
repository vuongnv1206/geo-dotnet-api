using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using FSH.WebApi.Application.TeacherGroup.QRCode;

namespace FSH.WebApi.Infrastructure.TeacherGroup.QrCodeRepo;
public class QrCode : IQrCode
{
    public string CreateQrCode(Dictionary<string, string> parameters, string? link)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters), "Parameters cannot be null");
        }

        // Generate the query string from parameters
        string queryString = GenerateQueryString(parameters);

        // Construct the final data to encode
        string dataToEncode = string.IsNullOrEmpty(link)
            ? queryString
            : $"{link}?{queryString}";

        // Generate QR code image and return base64 string
        return GenerateBase64QrCode(dataToEncode);
    }

    private static string GenerateQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
    }

    private static string GenerateBase64QrCode(string data)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);

            using (var qrCode = new QRCode(qrCodeData))
            using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
            }
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
