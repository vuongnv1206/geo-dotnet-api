using FSH.WebApi.Application.Examination.Services;
using FSH.WebApi.Application.Examination.Services.Models;
using Mapster;
using Xceed.Document.NET;
using Xceed.Words.NET;
using FSH.WebApi.Domain.Question.Enums;
using System.Text.RegularExpressions;

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
    private readonly ISerializerService _serializerService;
    private readonly IMediator _mediator;

    public GeneratePaperDocxRequestHandler(
        IPaperTemplateService paperTemplateService,
        ISerializerService serializerService,
        IMediator mediator)
    {
        _paperTemplateService = paperTemplateService;
        _serializerService = serializerService;
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
            Duration = paperDto.Duration.ToString(),
            SubjectName = paperDto.Subject.Name,
            TotalQuestion = paperDto.Questions.Count,
            Questions = paperDto.Questions.Adapt<List<QuestionModel>>(),
        };


        using (var ms = new MemoryStream())
        {
            using (var document = DocX.Create(ms))
            {
                var paragraph = document.InsertParagraph()
                                        .FontSize(10)
                                        .Italic()
                                        .SpacingAfter(15);

                // Append the title and center it
                paragraph.Append(paperTemplateModel.ExamName)
                    .FontSize(15)
                    .Bold()
                    .AppendLine();

                // Append the exam code and center it
                paragraph.Append($"(Exam Code: {paperTemplateModel.ExamCode})")
                    .FontSize(8)
                    .AppendLine();

                // Append the subject and details and center it
                paragraph
                    .InsertTabStopPosition(Alignment.center, 200f, TabStopPositionLeader.dot)
                    .InsertTabStopPosition(Alignment.right, 125f, TabStopPositionLeader.dot)
                    .Append($"Subject: {paperTemplateModel.SubjectName ?? "\t"} - Duration: {(string.IsNullOrEmpty(paperTemplateModel.Duration) ? "\t" : paperTemplateModel.Duration)} minute(s)")
                    .FontSize(10)
                    .Italic()
                    .AppendLine();

                // Append the time and center it with balanced dots
                paragraph
                    .InsertTabStopPosition(Alignment.left, 150f, TabStopPositionLeader.dot)
                    .InsertTabStopPosition(Alignment.right, 450f, TabStopPositionLeader.dot)
                    .Append($"Time: {paperTemplateModel.StartTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? new string('.', 30)} - {paperTemplateModel.EndTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? new string('.', 30)}")
                    .FontSize(10)
                    .Italic()
                    .AppendLine()
                    .SpacingAfter(10);

                paragraph.Alignment = Alignment.center;

                document.InsertParagraph("\t")
                    .InsertTabStopPosition(Alignment.center, 100f, TabStopPositionLeader.hyphen)
                    .SpacingAfter(20)
                    .Alignment = Alignment.center;

                var studentName = document.InsertParagraph()
                                            .FontSize(10)
                                            .SpacingAfter(15);

                studentName
                    .InsertTabStopPosition(Alignment.right, 200, TabStopPositionLeader.dot)
                    .Append("Student's first and last name:")
                    .Append("\t")
                    .AppendLine();

                studentName
                    .InsertTabStopPosition(Alignment.right, 200, TabStopPositionLeader.dot)
                    .Append("Identification number: ")
                    .Append("\t")
                    .AppendLine();

                studentName
                   .InsertTabStopPosition(Alignment.right, 200, TabStopPositionLeader.dot)
                   .Append("Class room: ")
                   .Append("\t")
                   .AppendLine();

                // Add questions
                foreach (var question in paperTemplateModel.Questions.OrderBy(x => x.RawIndex))
                {

                    // title question
                    var questionTitle = document.InsertParagraph($"{question.RawIndex + 1}.[{question.QuestionType}] {(question.QuestionType != QuestionType.Matching ? question.Content : string.Empty)}")
                            .FontSize(12)
                            .SpacingAfter(5);

                    switch (question.QuestionType)
                    {
                        case QuestionType.SingleChoice:
                        case QuestionType.MultipleChoice:
                        case QuestionType.ReadingQuestionPassage:
                            int wordNumber = 0;
                            foreach (var answer in question.Answers)
                            {
                                wordNumber += Regex.Matches(answer.Content, "[A-Za-z]+").Count;
                            }

                            // 1 row
                            if (wordNumber < 10)
                            {
                                var table = document.AddTable(1, question.Answers.Count);
                                table.Alignment = Alignment.center;
                                table.Design = TableDesign.None;

                                for (int i = 0; i < question.Answers.Count; i++)
                                {
                                    char prefix = (char)(65 + i);
                                    table.Rows[0].Cells[i].Paragraphs[0].Append($"{prefix}. {question.Answers[i].Content}").FontSize(12);
                                }

                                document.InsertTable(table);
                            }// table 2xcol
                            else if (wordNumber >= 10 && wordNumber < 15 && question.Answers.Count % 2 == 0)
                            {
                                var table = document.AddTable(2, question.Answers.Count / 2);
                                table.Alignment = Alignment.center;
                                table.Design = TableDesign.None;

                                for (int i = 0; i < question.Answers.Count; i++)
                                {
                                    char prefix = (char)(65 + i);
                                    table.Rows[i % 2].Cells[i / 2].Paragraphs[0].Append($"{prefix}. {question.Answers[i].Content}").FontSize(12);
                                }

                                document.InsertTable(table);
                            }
                            else
                            {
                                var table = document.AddTable(question.Answers.Count, 1);
                                table.Alignment = Alignment.center;
                                table.Design = TableDesign.None;

                                for (int i = 0; i < question.Answers.Count; i++)
                                {
                                    char prefix = (char)(65 + i);
                                    var cellParagraph = table.Rows[i].Cells[0].Paragraphs[0];
                                    cellParagraph.Append($"{prefix}. {question.Answers[i].Content}").FontSize(12);
                                }

                                document.InsertTable(table);
                            }

                            break;
                        case QuestionType.FillBlank:
                            Func<string, string> replaceFunction = (match) =>
                            {
                                return "............ ";
                            };

                            var functionReplaceTextOptions = new FunctionReplaceTextOptions()
                            {
                                FindPattern = @"\$_fillblank\[\d+\]",
                                RegexMatchHandler = replaceFunction,
                                RegExOptions = RegexOptions.IgnoreCase
                            };

                            questionTitle.ReplaceText(functionReplaceTextOptions);

                            break;
                        case QuestionType.Writing:
                            document
                            .InsertParagraph("\t\n\t\n\t\n\t\n\t")
                            .InsertTabStopPosition(Alignment.left, 500, TabStopPositionLeader.dot)
                            .SpacingAfter(8)
                            .Alignment = Alignment.center;
                            break;
                        case QuestionType.Reading:
                            int questionNumber = 1;
                            foreach (var questionPassage in question.QuestionPassages)
                            {
                                var questionPassageDoc = document
                                    .InsertParagraph()
                                    .Append($"\t{questionNumber}. {questionPassage.Content}")
                                    .SpacingAfter(5);

                                int answerNumber = 0;
                                foreach (var ans in questionPassage.Answers)
                                {
                                    questionPassageDoc
                                        .AppendLine($"{(char)(65 + answerNumber)}. {ans.Content}");
                                    answerNumber++;
                                }

                                questionNumber++;
                                questionPassageDoc.IndentationHanging = 50;
                            }

                            break;
                        case QuestionType.Matching:
                            var questionMatching = _serializerService.Deserialize<QuestionMatchingData>(question.Content);
                            questionTitle.Append($"{questionMatching.Question}").AppendLine();

                            var tableMatching = document.AddTable(questionMatching.ColumnA.Count, 2);
                            tableMatching.Alignment = Alignment.center;

                            int rowIndex = 0;
                            foreach (string key in questionMatching.ColumnA.Keys)
                            {
                                tableMatching.Rows[rowIndex].Cells[0].Paragraphs[0].Append($"{key}. {questionMatching.ColumnA[key]}");
                                tableMatching.Rows[rowIndex].Cells[1].Paragraphs[0].Append($"{(char)(65 + rowIndex)}. {questionMatching.ColumnB[key]}");
                                rowIndex++;
                            }

                            document.InsertTable(tableMatching);
                            var studentAnswer = document.InsertParagraph();
                            rowIndex = 0;
                            foreach (string key in questionMatching.ColumnA.Keys)
                            {
                                studentAnswer
                                    .Append($"{key}")
                                    .Append("\t")
                                    .InsertTabStopPosition(Alignment.left, 50 * (rowIndex + 1), TabStopPositionLeader.dot)
                                    .SpacingBefore(5);
                                rowIndex++;
                            }

                            break;
                    }

                    document.InsertParagraph()
                        .SpacingAfter(8);
                }

                // Save the document
                document.Save();
            }

            return ms.ToArray();
        }
    }
}
