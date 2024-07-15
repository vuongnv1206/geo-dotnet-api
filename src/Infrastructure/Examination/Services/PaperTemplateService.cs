using FSH.WebApi.Application.Examination.Services;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Infrastructure.Examination.Services;
public class PaperTemplateService : IPaperTemplateService
{
    public string GeneratePaperTemplate<T>(string templateName, T paperTemplateModel)
    {
        // Lấy nội dung template từ tên template
        string template = GetTemplate(templateName);
        // Tạo một đối tượng RazorEngine để biên dịch template
        IRazorEngine razorEngine = new RazorEngine();
        // Biên dịch template thành một template đã biên dịch
        IRazorEngineCompiledTemplate modifiedTemplate = razorEngine.Compile(template);
        // Chạy template đã biên dịch với dữ liệu cung cấp và trả về kết quả dưới dạng chuỗi
        return modifiedTemplate.Run(paperTemplateModel);
    }
    public static string GetTemplate(string templateName)
    {
        // Lấy thư mục gốc của ứng dụng
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        // Tạo đường dẫn đến thư mục chứa các template
        string tmplFolder = Path.Combine(baseDirectory, "PaperTemplates");
        // Tạo đường dẫn đầy đủ đến file template dựa trên tên template
        string filePath = Path.Combine(tmplFolder, $"{templateName}.cshtml");
        // Mở file template để đọc nội dung
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        // Sử dụng StreamReader để đọc nội dung của file
        using var sr = new StreamReader(fs, Encoding.Default);
        // Đọc toàn bộ nội dung của file và lưu vào biến content
        string content = sr.ReadToEnd();
        sr.Close();
        // Trả về nội dung của file dưới dạng chuỗi
        return content;
    }

}
