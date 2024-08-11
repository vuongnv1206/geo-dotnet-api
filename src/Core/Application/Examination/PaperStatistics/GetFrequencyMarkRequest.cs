
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetFrequencyMarkRequest : IRequest<ClassroomFrequencyMarkDto>
{
    public Guid PaperId { get; set; }
    public Guid? classroomId { get; set; }
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

        if (request.classroomId.HasValue)
        {
            classroom = await _repoClass.FirstOrDefaultAsync(new ClassByIdSpec(request.classroomId.Value,currentUserId))
                ?? throw new NotFoundException(_t["Classroom {0} Not Found.", request.classroomId]);

            submissions.AddRange(paper.SubmitPapers.Where(sp => classroom.UserClasses.Any(uc => uc.Student.StId == sp.CreatedBy)));

        }else
        {
            submissions.AddRange(paper.SubmitPapers);
        }



        var totalRegister = submissions.Count;
        var totalAttendee = submissions.Count(s => s.Status == SubmitPaperStatus.End);
        var frequencyMarks = new List<FrequencyMarkDto>();

        // Loop through each point from 0 to 9
        for (int i = 0; i < 10; i++)
        {
            var fromMark = i; // Từ điểm hiện tại
            var toMark = i + 1; // Đến điểm tiếp theo
            var count = submissions.Count(s => s.TotalMark >= fromMark && s.TotalMark < toMark);
            var rate = totalAttendee > 0 ? (float)count / totalAttendee * 100 : 0;

            frequencyMarks.Add(new FrequencyMarkDto
            {
                FromMark = fromMark,
                ToMark = toMark,
                Total = count,
                Rate = rate
            });
        }

        // Xử lý riêng cho điểm 10
        var countMax = submissions.Count(s => s.TotalMark == 10);
        frequencyMarks.Add(new FrequencyMarkDto
        {
            FromMark = 10,
            ToMark = 10,
            Total = countMax,
            Rate = totalAttendee > 0 ? (float)countMax / totalAttendee * 100 : 0,
        });



        return new ClassroomFrequencyMarkDto
        {
            ClassName = classroom.Name,
            TotalRegister = totalRegister,
            TotalAttendee = totalAttendee,
            FrequencyMarks = frequencyMarks
        };


    }
}

