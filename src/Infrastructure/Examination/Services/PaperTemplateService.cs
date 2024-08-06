using ClosedXML.Excel;
using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using FSH.WebApi.Application.Examination.PaperStatistics;
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

    public byte[] GenerateFrequencyMarkSheet(List<ClassroomFrequencyMarkDto> data)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Thống kê phổ điểm");
            worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(1, 1).Value = "Lớp";
            worksheet.Cell(1, 2).Value = "Số học sinh";
            worksheet.Range(1, 2, 1, 3).Merge().Value = "Số học sinh";
            worksheet.Cell(2, 2).Value = "Đăng ký";
            worksheet.Cell(2, 3).Value = "Dự thi";

            var intervals = new List<float>();
            foreach (var classData in data)
            {
                foreach (var mark in classData.FrequencyMarks)
                {
                    if (!intervals.Contains(mark.ToMark))
                    {
                        intervals.Add(mark.ToMark);
                    }
                }
            }

            intervals.Sort();

            int colIndex = 4;
            for (int i = 0; i < intervals.Count; i++)
            {
                var interval = intervals[i];
                worksheet.Cell(1, colIndex).Value = $"<={interval}";
                worksheet.Range(1, colIndex, 1, colIndex + 1).Merge().Value = $"<{interval}";
                worksheet.Cell(2, colIndex).Value = "SL";
                worksheet.Cell(2, colIndex + 1).Value = "%";
                colIndex += 2;
            }

            int row = 3;

            foreach (var classData in data)
            {
                worksheet.Cell(row, 1).Value = classData.ClassName;
                worksheet.Cell(row, 2).Value = classData.TotalRegister;
                worksheet.Cell(row, 3).Value = classData.TotalAttendee;

                colIndex = 4;
                foreach (var interval in intervals)
                {
                    var frequencyMark = classData.FrequencyMarks.FirstOrDefault(fm => fm.ToMark == interval);
                    worksheet.Cell(row, colIndex).Value = frequencyMark?.Total ?? 0;
                    worksheet.Cell(row, colIndex + 1).Value = frequencyMark?.Rate ?? 0;
                    colIndex += 2;
                }

                row++;
            }

            AddTotalRow(worksheet, row, intervals);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }

    public byte[] GenerateTranscriptSheet(TranscriptPaginationResponse data)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Transcripts");

            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 2).Value = "Học sinh";
            worksheet.Cell(1, 3).Value = "Lớp";
            worksheet.Cell(1, 4).Value = "Điểm số";
            worksheet.Cell(1, 5).Value = "Bắt đầu làm bài";
            worksheet.Cell(1, 6).Value = "Kết thúc bài làm";

            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            int row = 2;
            int index = 1;

            foreach (var transcript in data.Data)
            {
                worksheet.Cell(row, 1).Value = index++;
                worksheet.Cell(row, 2).Value = transcript.Attendee != null
                    ? $"{transcript.Attendee.FirstName ?? ""} {transcript.Attendee.LastName ?? ""}"
                    : "Thông tin học sinh không có";

                worksheet.Cell(row, 3).Value = transcript.Classrooms != null ? string.Join(", ", transcript.Classrooms.Select(x => x.Name)) : "Thí sinh tự do";
                worksheet.Cell(row, 4).Value = transcript.Mark;
                worksheet.Cell(row, 5).Value = transcript.StartedTest.ToString("dd/MM/yyyy HH:mm:ss");
                worksheet.Cell(row, 6).Value = transcript.FinishedTest?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa hoàn thành";

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }

    public byte[] GeneratePaperInfoSheet(PaperInfoStatistic data)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Paper Info");

            worksheet.Cell(1, 1).Value = "Exam Name";
            worksheet.Cell(1, 2).Value = "Total Register";
            worksheet.Cell(1, 3).Value = "Total Attendee";
            worksheet.Cell(1, 4).Value = "Total Not Complete";

            worksheet.Cell(2, 1).Value = data.ExamName;
            worksheet.Cell(2, 2).Value = data.TotalRegister;
            worksheet.Cell(2, 3).Value = data.TotalAttendee;
            worksheet.Cell(2, 4).Value = data.TotalNotComplete;

            var headerRange = worksheet.Range(1, 1, 1, 4);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }

    private void AddTotalRow(IXLWorksheet worksheet, int row, List<float> intervals)
    {
        worksheet.Cell(row, 1).Value = "TỔNG";
        worksheet.Range(row, 1, row, 3).Merge();

        for (int i = 4; i < 4 + intervals.Count * 2; i += 2)
        {
            worksheet.Cell(row, i).FormulaA1 = $"SUM({worksheet.Cell(3, i).Address}:{worksheet.Cell(row - 1, i).Address})";
            worksheet.Cell(row, i + 1).FormulaA1 = $"AVERAGE({worksheet.Cell(3, i + 1).Address}:{worksheet.Cell(row - 1, i + 1).Address})";
        }
    }

}
