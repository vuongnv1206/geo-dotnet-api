// using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
// using OfficeOpenXml;


// namespace FSH.WebApi.Application.Examination.PaperStatistics;
// public class GeneratePaperStatisticExcelRequest : IRequest<byte[]>
// {
//    public Guid PaperId { get; set; }
//    public List<RequestStatisticType> RequestStatisticTypes { get; set; }
// }

// public class GeneratePaperStatisticExcelRequestHandler : IRequestHandler<GeneratePaperStatisticExcelRequest, byte[]>
// {
//    private readonly IMediator _mediator;

// public GeneratePaperStatisticExcelRequestHandler(IMediator mediator)
//    {
//        _mediator = mediator;
//    }

// public async Task<byte[]> Handle(GeneratePaperStatisticExcelRequest request, CancellationToken cancellationToken)
//    {
//        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// using (var package = new ExcelPackage())
//        {
//            foreach (var requestType in request.RequestStatisticTypes)
//            {
//                switch (requestType)
//                {
//                    case RequestStatisticType.GetClassroomFrequencyMark:
//                        var frequencyMarkData = await _mediator.Send(new GetClassroomFrequencyMarkRequest { PaperId = request.PaperId }, cancellationToken);
//                        AddFrequencyMarkSheet(package, frequencyMarkData);
//                        break;

// case RequestStatisticType.GetListTranscript:
//                        var transcriptData = await _mediator.Send(new GetListTranscriptRequest { PaperId = request.PaperId }, cancellationToken);
//                        AddTranscriptSheet(package, transcriptData);
//                        break;

// case RequestStatisticType.GetPaperInfo:
//                        var paperInfoData = await _mediator.Send(new GetPaperInfoRequest(request.PaperId, null), cancellationToken);
//                        AddPaperInfoSheet(package, paperInfoData);
//                        break;

// default:
//                        throw new ArgumentException("Invalid request type");
//                }
//            }

// return package.GetAsByteArray();
//        }
//    }

// private void AddFrequencyMarkSheet(ExcelPackage package, List<ClassroomFrequencyMarkDto> data)
//    {
//        var worksheet = package.Workbook.Worksheets.Add("Thống kê phổ điểm");
//        worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//        // Đặt tiêu đề cho các cột
//        worksheet.Cells[1, 1].Value = "Lớp";
//        worksheet.Cells[1, 2].Value = "Số học sinh";
//        worksheet.Cells[1, 2, 1, 3].Merge = true;
//        worksheet.Cells[2, 2].Value = "Đăng ký";
//        worksheet.Cells[2, 3].Value = "Dự thi";

// // Tạo danh sách các khoảng điểm từ dữ liệu
//        var intervals = new List<float>();
//        foreach (var classData in data)
//        {
//            foreach (var mark in classData.FrequencyMarks)
//            {
//                if (!intervals.Contains(mark.ToMark))
//                {
//                    intervals.Add(mark.ToMark);
//                }
//            }
//        }

// // Sắp xếp các khoảng điểm
//        intervals.Sort();

// // Đặt tiêu đề cho các cột khoảng điểm
//        int colIndex = 4;
//        for (int i = 0; i < intervals.Count; i++)
//        {
//            var interval = intervals[i];
//            if (i == intervals.Count - 1)
//            {
//                worksheet.Cells[1, colIndex].Value = $"<={interval}";
//            }
//            else
//            {
//                worksheet.Cells[1, colIndex].Value = $"<{interval}";
//            }
//            worksheet.Cells[1, colIndex, 1, colIndex + 1].Merge = true;
//            worksheet.Cells[2, colIndex].Value = "SL";
//            worksheet.Cells[2, colIndex + 1].Value = "%";
//            colIndex += 2;
//        }

// int row = 3;

// foreach (var classData in data)
//        {
//            worksheet.Cells[row, 1].Value = classData.ClassName;
//            worksheet.Cells[row, 2].Value = classData.TotalRegister;
//            worksheet.Cells[row, 3].Value = classData.TotalAttendee;

// // Điền dữ liệu cho các khoảng điểm
//            colIndex = 4;
//            foreach (var interval in intervals)
//            {
//                var frequencyMark = classData.FrequencyMarks.FirstOrDefault(fm => fm.ToMark == interval);
//                if (frequencyMark != null)
//                {
//                    worksheet.Cells[row, colIndex].Value = frequencyMark.Total;
//                    worksheet.Cells[row, colIndex + 1].Value = frequencyMark.Rate;
//                }
//                else
//                {
//                    worksheet.Cells[row, colIndex].Value = 0;
//                    worksheet.Cells[row, colIndex + 1].Value = 0;
//                }
//                colIndex += 2;
//            }

// row++;
//        }

// // Thêm các hàng tổng cộng nếu cần
//        AddTotalRow(worksheet, row, intervals);
//    }

// private void AddTotalRow(ExcelWorksheet worksheet, int row, List<float> intervals)
//    {
//        worksheet.Cells[row, 1].Value = "TỔNG";
//        worksheet.Cells[row, 1, row, 3].Merge = true;

// // Tính tổng cho các cột số lượng và tỷ lệ phần trăm
//        for (int i = 4; i < 4 + intervals.Count * 2; i += 2)
//        {
//            worksheet.Cells[row, i].Formula = $"SUM({worksheet.Cells[3, i].Address}:{worksheet.Cells[row - 1, i].Address})";
//            worksheet.Cells[row, i + 1].Formula = $"AVERAGE({worksheet.Cells[3, i + 1].Address}:{worksheet.Cells[row - 1, i + 1].Address})";
//        }
//    }

// private void AddTranscriptSheet(ExcelPackage package, TranscriptPaginationResponse data)
//    {
//        var worksheet = package.Workbook.Worksheets.Add("Transcripts");

// // Đặt tiêu đề cho các cột
//        worksheet.Cells[1, 1].Value = "STT";
//        worksheet.Cells[1, 2].Value = "Học sinh";
//        worksheet.Cells[1, 3].Value = "Lớp";
//        worksheet.Cells[1, 4].Value = "Điểm số";
//        worksheet.Cells[1, 5].Value = "Bắt đầu làm bài";
//        worksheet.Cells[1, 6].Value = "Kết thúc bài làm";

// // Định dạng tiêu đề
//        using (var range = worksheet.Cells[1, 1, 1, 6])
//        {
//            range.Style.Font.Bold = true;
//            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
//            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
//            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
//        }

// int row = 2;
//        int index = 1;

// foreach (var transcript in data.Data)
//        {
//            worksheet.Cells[row, 1].Value = index++;
//            worksheet.Cells[row, 2].Value = $"{transcript.Attendee.FirstName} {transcript.Attendee.LastName}";
//            worksheet.Cells[row, 3].Value = transcript.Classroom != null ? transcript.Classroom.Name : "Thí sinh tự do";
//            worksheet.Cells[row, 4].Value = transcript.Mark;
//            worksheet.Cells[row, 5].Value = transcript.StartedTest.ToString("dd/MM/yyyy HH:mm:ss");
//            worksheet.Cells[row, 6].Value = transcript.FinishedTest?.ToString("dd/MM/yyyy HH:mm:ss") ?? "Chưa hoàn thành";

// int colIndex = 7; // Bắt đầu cột câu hỏi từ cột thứ 7

// foreach (var questionResult in transcript.QuestionResults)
//            {
//                // Hiển thị câu hỏi chính
//                worksheet.Cells[row - 1, colIndex].Value = $"Câu {questionResult.RawIndex} ({questionResult.Score} Điểm)";
//                worksheet.Cells[row - 1, colIndex].Style.Font.Bold = true;

// // Chuỗi để chứa kết quả của các câu hỏi con
//                if (questionResult.QuestionPassages != null && questionResult.QuestionPassages.Any())
//                {
//                    string subQuestionContent = "";
//                    int subIndex = 1;
//                    foreach (var subQuestion in questionResult.QuestionPassages)
//                    {
//                        subQuestionContent += $"Câu {questionResult.RawIndex}.{subIndex} ({subQuestion.Score} Điểm): {subQuestion.IsCorrect}\n";
//                        subIndex++;
//                    }
//                    worksheet.Cells[row , colIndex].Value = subQuestionContent.Trim();
//                    worksheet.Cells[row , colIndex].Style.WrapText = true; // Gói văn bản trong ô
//                }

// colIndex++; // Chuyển sang cột câu hỏi tiếp theo
//            }

// row += 2; // Chuyển sang hàng mới cho học sinh tiếp theo
//        }

// // Tự động căn chỉnh độ rộng cột cho phù hợp với nội dung
//        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
//    }

// private void AddPaperInfoSheet(ExcelPackage package, PaperInfoStatistic data)
//    {
//        var worksheet = package.Workbook.Worksheets.Add("Paper Info");

// worksheet.Cells[1, 1].Value = "Exam Name";
//        worksheet.Cells[1, 2].Value = "Total Register";
//        worksheet.Cells[1, 3].Value = "Total Attendee";
//        worksheet.Cells[1, 4].Value = "Total Not Complete";

// worksheet.Cells[2, 1].Value = data.ExamName;
//        worksheet.Cells[2, 2].Value = data.TotalRegister;
//        worksheet.Cells[2, 3].Value = data.TotalAttendee;
//        worksheet.Cells[2, 4].Value = data.TotalNotComplete;
//    }
// }
