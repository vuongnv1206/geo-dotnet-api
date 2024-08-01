using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.UserStudents;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using Mapster;


namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetListTranscriptRequest : PaginationFilter, IRequest<TranscriptPaginationResponse>
{
    public Guid PaperId { get; set; }
    public Guid? ClassId { get; set; }
}


public class GetListTranscriptRequestHandler : IRequestHandler<GetListTranscriptRequest, TranscriptPaginationResponse>
{
    private readonly IRepository<Paper> _repoPaper;
    private readonly IRepository<Classes> _repoClass;
    private readonly IRepository<Student> _repoStudent;
    private readonly IRepository<SubmitPaper> _repoSubmitPaper;
    private readonly IStringLocalizer<GetListTranscriptRequestHandler> _t;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<QuestionClone> _questionCloneRepo;
    private readonly ISubmmitPaperService _submmitPaperService;
    public GetListTranscriptRequestHandler(
        IRepository<Paper> repoPaper,
        IRepository<Classes> repoClass,
        IRepository<Student> repoStudent,
        IRepository<SubmitPaper> repoSubmitPaper,
        IStringLocalizer<GetListTranscriptRequestHandler> t,
        ICurrentUser currentUser,
        IRepository<QuestionClone> questionCloneRepo,
        ISubmmitPaperService submmitPaperService
        )
    {
        _repoPaper = repoPaper;
        _repoClass = repoClass;
        _repoStudent = repoStudent;
        _repoSubmitPaper = repoSubmitPaper;
        _t = t;
        _currentUser = currentUser;
        _questionCloneRepo = questionCloneRepo;
        _submmitPaperService = submmitPaperService;
    }

    public async Task<TranscriptPaginationResponse> Handle(GetListTranscriptRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repoPaper.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken)
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var currentUserId = _currentUser.GetUserId();
        var submissions = new List<SubmitPaper>();
        int count = 0;

        var spec = new SubmitPaperBySearchSpec(request, Enumerable.Empty<Guid>());
        var x = await _repoSubmitPaper.ListAsync(spec, cancellationToken);
        count = await _repoSubmitPaper.CountAsync(spec, cancellationToken);
        submissions.AddRange(await _repoSubmitPaper.ListAsync(spec, cancellationToken));

        var questionsInPaper = paper.PaperQuestions.ToList();

        var results = new List<TranscriptResultDto>();
        foreach (var submission in submissions)
        {
            var questionResults = new List<QuestionResultDto>();

            foreach (var paperQuestion in questionsInPaper)
            {
                var studentAnswer = submission.SubmitPaperDetails.FirstOrDefault(a => a.QuestionId == paperQuestion.QuestionId);
                var questionResult = new QuestionResultDto
                {
                    RawIndex = paperQuestion.RawIndex.Value,
                    IsCorrect = studentAnswer != null ? (_submmitPaperService.IsCorrectAnswer(studentAnswer, paperQuestion.Question) ? "Đúng" : "Sai") : "Không trả lời",
                    Score = studentAnswer != null ? paperQuestion.Mark : 0
                };

                // Xử lý các câu hỏi Reading và thêm QuestionPassage nếu có
                if (paperQuestion.Question.QuestionType == Domain.Question.Enums.QuestionType.Reading)
                {
                    questionResult.QuestionPassages = new List<QuestionResultDto>();
                    await HandleReadingQuestion(paperQuestion.Question.QuestionPassages, paperQuestion.Mark, questionResult.QuestionPassages, submission.SubmitPaperDetails, cancellationToken);
                }
                else if (paperQuestion.Question.QuestionType == Domain.Question.Enums.QuestionType.Writing)
                {
                    questionResult.IsCorrect = "Không có thông tin";
                }

                questionResults.Add(questionResult);
            }

            var student = await _repoStudent.FirstOrDefaultAsync(new StudentByStIdSpec(submission.CreatedBy), cancellationToken);

            results.Add(new TranscriptResultDto
            {
                Attendee = student.Adapt<StudentDto>(),
                Mark = submission.TotalMark,
                Classrooms = paper.PaperAccesses
                                    .Where(x => x.Class.UserClasses
                                    .Any(uc => uc.Student.StId == submission.CreatedBy))
                                    .Select(x => x.Class).Adapt<List<ClassViewListDto>>(),
                StartedTest = submission.StartTime,
                FinishedTest = submission.EndTime,
                QuestionResults = questionResults // Thêm kết quả câu hỏi vào DTO
            });
        }

        float averageMark = results.Any() ? results.Average(r => r.Mark) : 0f;
        var paginatedResponse = new TranscriptPaginationResponse(
                 data: results,
                 count: count,
                 page: request.PageNumber,
                 pageSize: request.PageSize,
                 averageMark: averageMark
             );
        return paginatedResponse;
    }

    private async Task HandleReadingQuestion(List<QuestionClone> questionPassages, float mark, List<QuestionResultDto> questionResult, List<SubmitPaperDetail> studentAnswers, CancellationToken cancellationToken)
    {

        foreach (var passageQuestion in questionPassages)
        {
            var studentAnswer = studentAnswers.FirstOrDefault(a => a.QuestionId == passageQuestion.Id);
            var markOfQuestionPassage = mark / questionPassages.Count;
            var passageResult = new QuestionResultDto
            {
                IsCorrect = studentAnswer != null ? (_submmitPaperService.IsCorrectAnswer(studentAnswer, passageQuestion) ? "Đúng" : "Sai") : "Không trả lời",
                Score = studentAnswer != null ? markOfQuestionPassage : 0,
                QuestionPassages = new List<QuestionResultDto>()
            };

            // Đệ quy xử lý các câu hỏi con bên trong đoạn văn
            if (passageQuestion.QuestionPassages.Any())
            {
                await HandleReadingQuestion(passageQuestion.QuestionPassages, markOfQuestionPassage, passageResult.QuestionPassages, studentAnswers, cancellationToken);
            }

            questionResult.Add(passageResult);
        }
    }
}
