using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using OfficeOpenXml;


namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GeneratePaperStatisticExcelRequest : IRequest<byte[]>
{
    public Guid PaperId { get; set; }
    public List<RequestStatisticType> RequestStatisticTypes { get; set; }
}

public class GeneratePaperStatisticExcelRequestHandler : IRequestHandler<GeneratePaperStatisticExcelRequest, byte[]>
{
    private readonly IMediator _mediator;

    public GeneratePaperStatisticExcelRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<byte[]> Handle(GeneratePaperStatisticExcelRequest request, CancellationToken cancellationToken)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (var package = new ExcelPackage())
        {
            foreach (var requestType in request.RequestStatisticTypes)
            {
                switch (requestType)
                {
                    case RequestStatisticType.GetClassroomFrequencyMark:
                        var frequencyMarkData = await _mediator.Send(new GetClassroomFrequencyMarkRequest { PaperId = request.PaperId }, cancellationToken);
                        AddFrequencyMarkSheet(package, frequencyMarkData);
                        break;

                    case RequestStatisticType.GetListTranscript:
                        var transcriptData = await _mediator.Send(new GetListTranscriptRequest { PaperId = request.PaperId }, cancellationToken);
                        AddTranscriptSheet(package, transcriptData);
                        break;

                    case RequestStatisticType.GetQuestionStatistic:
                        var questionStatisticData = await _mediator.Send(new GetQuestionStatisticRequest { PaperId = request.PaperId }, cancellationToken);
                        AddQuestionStatisticSheet(package, questionStatisticData);
                        break;

                    default:
                        throw new ArgumentException("Invalid request type");
                }
            }

            return package.GetAsByteArray();
        }
    }

    private void AddFrequencyMarkSheet(ExcelPackage package, List<ClassroomFrequencyMarkDto> data)
    {
        var worksheet = package.Workbook.Worksheets.Add("Thống kê phổ điểm");
        worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        // Đặt tiêu đề cho các cột
        worksheet.Cells[1, 1].Value = "Lớp";
        worksheet.Cells[1, 2].Value = "Số học sinh";
        worksheet.Cells[1, 2, 1, 3].Merge = true;
        worksheet.Cells[2, 2].Value = "Đăng ký";
        worksheet.Cells[2, 3].Value = "Dự thi";

        // Tạo danh sách các khoảng điểm từ dữ liệu
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

        // Sắp xếp các khoảng điểm
        intervals.Sort();

        // Đặt tiêu đề cho các cột khoảng điểm
        int colIndex = 4;
        for (int i = 0; i < intervals.Count; i++)
        {
            var interval = intervals[i];
            if (i == intervals.Count - 1)
            {
                worksheet.Cells[1, colIndex].Value = $"<={interval}";
            }
            else
            {
                worksheet.Cells[1, colIndex].Value = $"<{interval}";
            }
            worksheet.Cells[1, colIndex, 1, colIndex + 1].Merge = true;
            worksheet.Cells[2, colIndex].Value = "SL";
            worksheet.Cells[2, colIndex + 1].Value = "%";
            colIndex += 2;
        }

        int row = 3;

        foreach (var classData in data)
        {
            worksheet.Cells[row, 1].Value = classData.ClassName;
            worksheet.Cells[row, 2].Value = classData.TotalRegister;
            worksheet.Cells[row, 3].Value = classData.TotalAttendee;

            // Điền dữ liệu cho các khoảng điểm
            colIndex = 4;
            foreach (var interval in intervals)
            {
                var frequencyMark = classData.FrequencyMarks.FirstOrDefault(fm => fm.ToMark == interval);
                if (frequencyMark != null)
                {
                    worksheet.Cells[row, colIndex].Value = frequencyMark.Total;
                    worksheet.Cells[row, colIndex + 1].Value = frequencyMark.Rate;
                }
                else
                {
                    worksheet.Cells[row, colIndex].Value = 0;
                    worksheet.Cells[row, colIndex + 1].Value = 0;
                }
                colIndex += 2;
            }

            row++;
        }

        // Thêm các hàng tổng cộng nếu cần
        AddTotalRow(worksheet, row, intervals);
    }

    private void AddTotalRow(ExcelWorksheet worksheet, int row, List<float> intervals)
    {
        worksheet.Cells[row, 1].Value = "TỔNG";
        worksheet.Cells[row, 1, row, 3].Merge = true;

        // Tính tổng cho các cột số lượng và tỷ lệ phần trăm
        for (int i = 4; i < 4 + intervals.Count * 2; i += 2)
        {
            worksheet.Cells[row, i].Formula = $"SUM({worksheet.Cells[3, i].Address}:{worksheet.Cells[row - 1, i].Address})";
            worksheet.Cells[row, i + 1].Formula = $"AVERAGE({worksheet.Cells[3, i + 1].Address}:{worksheet.Cells[row - 1, i + 1].Address})";
        }
    }

    private void AddTranscriptSheet(ExcelPackage package, TranscriptPaginationResponse data)
    {
        var worksheet = package.Workbook.Worksheets.Add("Transcripts");

        // Set up headers
        worksheet.Cells[1, 1].Value = "STT";
        worksheet.Cells[1, 2].Value = "Học sinh";
        worksheet.Cells[1, 3].Value = "Lớp";
        worksheet.Cells[1, 4].Value = "Điểm số";
        worksheet.Cells[1, 5].Value = "Bắt đầu làm bài";
        worksheet.Cells[1, 6].Value = "Kết thúc bài làm";

        // Format headers
        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        }

        // Fill in data
        int row = 2;
        int index = 1;

        foreach (var transcript in data.Data)
        {
            worksheet.Cells[row, 1].Value = index++;
            worksheet.Cells[row, 2].Value = $"{transcript.Attendee.FirstName} {transcript.Attendee.LastName}";
            worksheet.Cells[row, 3].Value = transcript.Classrooms != null && transcript.Classrooms.Any() ? string.Join(", ", transcript.Classrooms.Select(x => x.Name)) : "Thí sinh tự do";
            worksheet.Cells[row, 4].Value = transcript.Mark;
            worksheet.Cells[row, 5].Value = transcript.StartedTest.ToString("dd/MM/yyyy HH:mm:ss");
            worksheet.Cells[row, 6].Value = transcript.FinishedTest?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa hoàn thành";

            row++;
        }

        // Add average mark row
        worksheet.Cells[row, 3].Value = "Điểm trung bình:";
        worksheet.Cells[row, 4].Value = data.AverageMark;

        // Format average mark row
        using (var range = worksheet.Cells[row, 3, row, 4])
        {
            range.Style.Font.Bold = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        }

        // Auto fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
    }




    private void AddQuestionStatisticSheet(ExcelPackage package, PaginationResponse<QuestionStatisticDto> data)
    {
        var worksheet = package.Workbook.Worksheets.Add("Question Statistic");

        // Tạo tiêu đề cho các cột
        worksheet.Cells[1, 1].Value = "ID";
        worksheet.Cells[1, 2].Value = "Content";
        worksheet.Cells[1, 3].Value = "Question Type";
        worksheet.Cells[1, 4].Value = "Total Test";
        worksheet.Cells[1, 5].Value = "Total Answered";
        worksheet.Cells[1, 6].Value = "Total Correct";
        worksheet.Cells[1, 7].Value = "Total Wrong";
        worksheet.Cells[1, 8].Value = "Total Not Answered";
        worksheet.Cells[1, 9].Value = "Wrong Students";

        // Đặt định dạng cho các tiêu đề
        using (var range = worksheet.Cells[1, 1, 1, 11])
        {
            range.Style.Font.Bold = true;
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        int row = 2;

        foreach (var question in data.Data)
        {
            worksheet.Cells[row, 1].Value = question.Id;
            worksheet.Cells[row, 2].Value = question.Content;
            worksheet.Cells[row, 3].Value = question.QuestionType.ToString();
            worksheet.Cells[row, 4].Value = question.TotalTest;
            worksheet.Cells[row, 5].Value = question.TotalAnswered;
            worksheet.Cells[row, 6].Value = question.TotalCorrect;
            worksheet.Cells[row, 7].Value = question.TotalWrong;
            worksheet.Cells[row, 8].Value = question.TotalNotAnswered;

            // Điền danh sách thí sinh làm sai vào một cột
            if (question.WrongStudents != null && question.WrongStudents.Any())
            {
                var wrongStudentNames = question.WrongStudents.Select(s => $"{s.FirstName} {s.LastName}");
                worksheet.Cells[row, 9].Value = string.Join(", ", wrongStudentNames);
            }
            // Điền các câu hỏi con (QuestionPassages) nếu có
            if (question.QuestionPassages != null && question.QuestionPassages.Any())
            {
                foreach (var passage in question.QuestionPassages)
                {
                    row++;
                    worksheet.Cells[row, 1].Value = passage.Id;
                    worksheet.Cells[row, 2].Value = "   " + passage.Content; // Thụt lề để dễ nhận biết là câu hỏi con
                    worksheet.Cells[row, 3].Value = passage.QuestionType.ToString();
                    worksheet.Cells[row, 4].Value = passage.TotalTest;
                    worksheet.Cells[row, 5].Value = passage.TotalAnswered;
                    worksheet.Cells[row, 6].Value = passage.TotalCorrect;
                    worksheet.Cells[row, 7].Value = passage.TotalWrong;
                    worksheet.Cells[row, 8].Value = passage.TotalNotAnswered;
                    // Điền danh sách thí sinh làm sai cho câu hỏi con
                    if (passage.WrongStudents != null && passage.WrongStudents.Any())
                    {
                        var wrongStudentNames = passage.WrongStudents.Select(s => $"{s.FirstName} {s.LastName}");
                        worksheet.Cells[row, 9].Value = string.Join(", ", wrongStudentNames);
                    }
                }
            }

            row++;
        }

        // Đặt định dạng cho các cột
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
    }

}
