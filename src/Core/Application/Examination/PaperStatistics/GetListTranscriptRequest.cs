using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.UserStudents;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.PaperStatistics.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using Mapster;


namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetListTranscriptRequest : PaginationFilter, IRequest<TranscriptPaginationResponse>
{
    public Guid PaperId { get; set; }
    public Guid? ClassroomId { get; set; }
}


public class GetListTranscriptRequestHandler : IRequestHandler<GetListTranscriptRequest, TranscriptPaginationResponse>
{
    private readonly IRepository<Paper> _repoPaper;
    private readonly IRepository<Classes> _repoClass;
    private readonly IRepository<Student> _repoStudent;
    private readonly IRepository<SubmitPaper> _repoSubmitPaper;
    private readonly IStringLocalizer<GetListTranscriptRequestHandler> _t;
    private readonly ICurrentUser _currentUser;

    public GetListTranscriptRequestHandler(
        IRepository<Paper> repoPaper,
        IRepository<Classes> repoClass,
        IRepository<Student> repoStudent,
        IRepository<SubmitPaper> repoSubmitPaper,
        IStringLocalizer<GetListTranscriptRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repoPaper = repoPaper;
        _repoClass = repoClass;
        _repoStudent = repoStudent;
        _repoSubmitPaper = repoSubmitPaper;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<TranscriptPaginationResponse> Handle(GetListTranscriptRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repoPaper.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);
        var currentUserId = _currentUser.GetUserId();
        List<SubmitPaper> submissions = new List<SubmitPaper>();
        var count = 0;
        if (request.ClassroomId.HasValue)
        {
            var classroom = await _repoClass.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassroomId.Value, _currentUser.GetUserId()), cancellationToken);
            var studentIdsInClass = classroom.GetStudentIds();
            var spec = new SubmitPaperBySearchSpec(request, studentIdsInClass);
            count = await _repoSubmitPaper.CountAsync(spec, cancellationToken);
            submissions.AddRange(await _repoSubmitPaper.ListAsync(spec, cancellationToken));
        }
        else
        {
            var spec = new SubmitPaperBySearchSpec(request, Enumerable.Empty<Guid>());
            count = await _repoSubmitPaper.CountAsync(spec, cancellationToken);
            submissions.AddRange(await _repoSubmitPaper.ListAsync(spec, cancellationToken));
        }

        var results = new List<TranscriptResultDto>();
        foreach (var submission in submissions)
        {
            var student = await _repoStudent.FirstOrDefaultAsync(new StudentByStIdSpec(submission.CreatedBy), cancellationToken);
            results.Add(new TranscriptResultDto
            {
                Attendee = student.Adapt<StudentDto>(),
                Mark = submission.TotalMark,
                StartedTest = submission.StartTime,
                FinishedTest = submission.EndTime
            });
        }

        var averageMark = results.Any() ? results.Average(r => r.Mark) : 0f;
        var paginatedResponse = new TranscriptPaginationResponse(
                 data: results,
                 count: count,
                 page: request.PageNumber,
                 pageSize: request.PageSize,
                 averageMark: averageMark
             );
        return paginatedResponse;
    }
}