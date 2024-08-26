using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.Specs;
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

            ClassViewListDto foundClass = null;

            foreach (var pa in paper.PaperAccesses)
            {
                if (pa.ClassId.HasValue && pa.Class != null && pa.Class.UserClasses
                        .Any(uc => uc.Student.StId == submission.CreatedBy))
                {
                    foundClass = pa.Class.Adapt<ClassViewListDto>();
                    break; // Dừng lại khi tìm thấy lớp học
                }
                else if (pa.UserId.HasValue)
                {
                    var classForUser = await GetClassByStudentId(pa.UserId.Value);
                    if (classForUser.UserClasses.Any(x => x.Student.StId == submission.CreatedBy))
                    {
                        foundClass = classForUser.Adapt<ClassViewListDto>();
                        break; // Dừng lại khi tìm thấy lớp học từ UserId
                    }
                }
            }

            var student = await _repoStudent.FirstOrDefaultAsync(new StudentByStIdSpec(submission.CreatedBy), cancellationToken);

            results.Add(new TranscriptResultDto
            {
                Attendee = student != null ? student.Adapt<StudentDto>() : null,
                Mark = submission.TotalMark,
                Classrooms = foundClass != null ? new List<ClassViewListDto> { foundClass } : new List<ClassViewListDto>(),
                StartedTest = submission.StartTime,
                FinishedTest = submission.EndTime
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

    private async Task<Classes> GetClassByStudentId(Guid studentId)
    {
        var specClass = new ClassByStudentClassIdSpec(studentId);
        var classroom = await _repoClass.FirstOrDefaultAsync(specClass)
            ?? throw new NotFoundException(_t["Classroom {0} Not Found."]);

        return classroom;
    }
}
