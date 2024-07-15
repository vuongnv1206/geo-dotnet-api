using FSH.WebApi.Application.Examination.Services;
using FSH.WebApi.Application.Examination.Services.Models;
using Mapster;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Microsoft.AspNetCore.Http;

namespace FSH.WebApi.Application.Examination.Papers;
public class GeneratePaperDocxRequest : IRequest<byte[]>
{
    public Guid PaperId { get; set; }
    public GeneratePaperDocxRequest(Guid paperId)
    {
        PaperId = paperId;
    }
}


public class GeneratePaperDocxRequestHandler : IRequestHandler<GeneratePaperDocxRequest, byte[]>
{
    private readonly IPaperTemplateService _paperTemplateService;
    private readonly IMediator _mediator;

    public GeneratePaperDocxRequestHandler(IPaperTemplateService paperTemplateService,IMediator mediator)
    {
        _paperTemplateService = paperTemplateService;
        _mediator = mediator;
    }

    public async Task<byte[]> Handle(GeneratePaperDocxRequest request, CancellationToken cancellationToken)
    {

        var getPaperByIdRequest = new GetPaperByIdRequest(request.PaperId);
        var paperDto = await _mediator.Send(getPaperByIdRequest, cancellationToken);
        var paperTemplateModel = new PaperTemplateModel
        {
            ExamCode = paperDto.ExamCode,
            ExamName = paperDto.ExamName,
            StartTime = paperDto.StartTime,
            EndTime = paperDto.EndTime,
            Duration = paperDto.Duration,
            SubjectName = paperDto.Subject?.Name,
            TotalQuestion = paperDto.Questions.Count,
            Questions = paperDto.Questions.Adapt<List<QuestionModel>>(),
        };


        using (var ms = new MemoryStream())
        {
            using (var document = DocX.Create(ms))
            {
                // Add title
                document.InsertParagraph(paperTemplateModel.ExamName)
                        .FontSize(20)
                        .Bold()
                        .Alignment = Alignment.center;

                // Add exam details

                document.InsertParagraph($"Subject: {paperTemplateModel.SubjectName}")
                        .FontSize(12)
                        .SpacingAfter(10);
                document.InsertParagraph($"Exam Code: {paperTemplateModel.ExamCode}")
                        .FontSize(12)
                        .SpacingAfter(10);
                document.InsertParagraph($"Start Time: {paperTemplateModel.StartTime?.ToString("yyyy-MM-dd HH:mm:ss")}")
                        .FontSize(12)
                        .SpacingAfter(10);
                document.InsertParagraph($"End Time: {paperTemplateModel.EndTime?.ToString("yyyy-MM-dd HH:mm:ss")}")
                        .FontSize(12)
                        .SpacingAfter(10);
                document.InsertParagraph($"Duration: {paperTemplateModel.Duration} minutes")
                        .FontSize(12)
                        .SpacingAfter(20);

                // Add questions
                foreach (var question in paperTemplateModel.Questions)
                {
                    document.InsertParagraph($"{question.RawIndex}. {question.Content}")
                            .FontSize(12)
                            .SpacingAfter(10);

                    for (int i = 0; i < question.Answers.Count; i += 2)
                    {
                        var row = document.InsertParagraph();
                        row.Append($"A. {question.Answers[i].Content}").FontSize(12).SpacingAfter(5);
                        if (i + 1 < question.Answers.Count)
                        {
                            row.Append($"  B. {question.Answers[i + 1].Content}").FontSize(12).SpacingAfter(5);
                        }
                    }
                }

                // Save the document
                document.Save();
            }

            return ms.ToArray();
        }
    }
}
