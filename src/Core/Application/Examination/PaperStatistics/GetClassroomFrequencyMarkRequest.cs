using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Specs;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetClassroomFrequencyMarkRequest : IRequest<List<ClassroomFrequencyMarkDto>>
{
    public Guid PaperId { get; set; }
    public Guid? ClassroomId { get; set; }
}

public class GetClassroomFrequencyMarkRequestHandler : IRequestHandler<GetClassroomFrequencyMarkRequest, List<ClassroomFrequencyMarkDto>>
{
    private readonly IRepository<Paper> _repoPaper;
    private readonly IRepository<Classes> _repoClass;
    private readonly IRepository<Student> _repoStudent;
    private readonly IRepository<SubmitPaper> _repoSubmitPaper;
    private readonly IStringLocalizer<GetClassroomFrequencyMarkRequestHandler> _t;
    private readonly ICurrentUser _currentUser;

    public GetClassroomFrequencyMarkRequestHandler(
        IRepository<Paper> repoPaper,
        IRepository<Classes> repoClass,
        IRepository<Student> repoStudent,
        IRepository<SubmitPaper> repoSubmitPaper,
        IStringLocalizer<GetClassroomFrequencyMarkRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repoPaper = repoPaper;
        _repoClass = repoClass;
        _repoStudent = repoStudent;
        _repoSubmitPaper = repoSubmitPaper;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<List<ClassroomFrequencyMarkDto>> Handle(GetClassroomFrequencyMarkRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repoPaper.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);
        var currentUserId = _currentUser.GetUserId();
        var classroom = new Classes();
        List<SubmitPaper> submissions = new List<SubmitPaper>();

        var accessibleStudentIds = new List<Guid>();
        //Filter theo classroom
        if (request.ClassroomId.HasValue)
        {
            classroom = await _repoClass.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassroomId.Value, currentUserId));
            var studentIdsInClass = classroom.GetStudentIds();
            accessibleStudentIds.AddRange(studentIdsInClass);
            submissions.AddRange(await _repoSubmitPaper.ListAsync(new SubmitPaperByUserIdsSpec(request.PaperId, accessibleStudentIds), cancellationToken));
        }
        else
        {
            submissions.AddRange(paper.SubmitPapers);
        }
    

        // Tạo một dictionary để lưu danh sách bài nộp theo lớp
        var classSubmissionsMap = new Dictionary<string, List<SubmitPaper>>();

        foreach (var submission in submissions)
        {
            // Tìm lớp của học sinh từ bảng Classes thông qua CreatedBy
            var studentClasses = await _repoClass.ListAsync(new ClassesByStudentIdSpec(submission.CreatedBy), cancellationToken);
            var classNames = studentClasses.Select(c => c.Name).ToList();

            // Nếu học sinh không thuộc lớp nào, gán "Thí sinh tự do"
            if (classNames.Count == 0)
            {
                classNames.Add("Thí sinh tự do");
            }

            // Nhóm bài nộp theo các lớp tìm được
            foreach (var className in classNames)
            {
                if (!classSubmissionsMap.ContainsKey(className))
                {
                    classSubmissionsMap[className] = new List<SubmitPaper>();
                }
                classSubmissionsMap[className].Add(submission);
            }
        }

        var classFrequencyMarks = new List<ClassroomFrequencyMarkDto>();

        // Tính toán tần số điểm cho từng lớp
        foreach (var classGroup in classSubmissionsMap)
        {
            var className = classGroup.Key;
            var classSubmissions = classGroup.Value;

            var totalRegister = classSubmissions.Count;
            var totalAttendee = classSubmissions.Count(s => s.Status == SubmitPaperStatus.End);
            var maxPointInPaper = paper.PaperQuestions.Sum(x => x.Mark);
            var interval = maxPointInPaper / 10.0;
            var frequencyMarks = new List<FrequencyMarkDto>();

            // Chia thang điểm thành 10 phần và tính tần số điểm cho từng khoảng
            for (int i = 0; i < 10; i++)
            {
                var fromMark = i * interval;
                var toMark = (i + 1) * interval;
                var count = classSubmissions.Count(s => s.TotalMark >= fromMark && s.TotalMark < toMark);
                var rate = totalAttendee > 0 ? (float)count / totalAttendee * 100 : 0;

                if (toMark == maxPointInPaper)
                {
                    var countMax = classSubmissions.Count(s => s.TotalMark >= fromMark && s.TotalMark <= toMark);
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


            // Thêm thông tin tần số điểm vào danh sách kết quả
            classFrequencyMarks.Add(new ClassroomFrequencyMarkDto
            {
                ClassName = className,
                TotalRegister = totalRegister,
                TotalAttendee = totalAttendee,
                FrequencyMarks = frequencyMarks
            });
        }

        // Trả về danh sách các tần số điểm theo lớp
        return classFrequencyMarks;
    }

}

