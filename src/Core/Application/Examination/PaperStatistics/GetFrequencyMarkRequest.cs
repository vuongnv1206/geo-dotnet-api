
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetFrequencyMarkRequest : IRequest<ClassroomFrequencyMarkDto>
{
    public Guid PaperId { get; set; }
}

public class GetFrequencyMarkRequestHandler : IRequestHandler<GetFrequencyMarkRequest, ClassroomFrequencyMarkDto>
{
    private readonly IRepository<Paper> _repoPaper;
    private readonly IRepository<Classes> _repoClass;
    private readonly IRepository<Student> _repoStudent;
    private readonly IRepository<SubmitPaper> _repoSubmitPaper;
    private readonly IStringLocalizer<GetFrequencyMarkRequestHandler> _t;
    private readonly ICurrentUser _currentUser;

    public GetFrequencyMarkRequestHandler(
        IRepository<Paper> repoPaper,
        IRepository<Classes> repoClass,
        IRepository<Student> repoStudent,
        IRepository<SubmitPaper> repoSubmitPaper,
        IStringLocalizer<GetFrequencyMarkRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repoPaper = repoPaper;
        _repoClass = repoClass;
        _repoStudent = repoStudent;
        _repoSubmitPaper = repoSubmitPaper;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<ClassroomFrequencyMarkDto> Handle(GetFrequencyMarkRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repoPaper.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);
        var currentUserId = _currentUser.GetUserId();
        var classroom = new Classes();
        List<SubmitPaper> submissions = new List<SubmitPaper>();

        var accessibleStudentIds = new List<Guid>();

        submissions.AddRange(paper.SubmitPapers);



        var totalRegister = submissions.Count;
        var totalAttendee = submissions.Count(s => s.Status == SubmitPaperStatus.End);
        var maxPointInPaper = paper.PaperQuestions.Sum(x => x.Mark);
        var interval = maxPointInPaper / 10.0;
        var frequencyMarks = new List<FrequencyMarkDto>();

        // Chia thang điểm thành 10 phần và tính tần số điểm cho từng khoảng
        for (int i = 0; i < 10; i++)
        {
            var fromMark = i * interval;
            var toMark = (i + 1) * interval;
            var count = submissions.Count(s => s.TotalMark >= fromMark && s.TotalMark < toMark);
            var rate = totalAttendee > 0 ? (float)count / totalAttendee * 100 : 0;

            if (toMark == maxPointInPaper)
            {
                var countMax = submissions.Count(s => s.TotalMark >= fromMark && s.TotalMark <= toMark);
                frequencyMarks.Add(new FrequencyMarkDto
                {
                    FromMark = (float)fromMark,
                    ToMark = (float)toMark,
                    Total = countMax,
                    Rate = totalAttendee > 0 ? (float)countMax / totalAttendee * 100 : 0,
                });
                break;
            }

            frequencyMarks.Add(new FrequencyMarkDto
            {
                FromMark = (float)fromMark,
                ToMark = (float)toMark,
                Total = count,
                Rate = rate
            });


        }

        return new ClassroomFrequencyMarkDto
        {
            TotalRegister = totalRegister,
            TotalAttendee = totalAttendee,
            FrequencyMarks = frequencyMarks
        };


    }
}

