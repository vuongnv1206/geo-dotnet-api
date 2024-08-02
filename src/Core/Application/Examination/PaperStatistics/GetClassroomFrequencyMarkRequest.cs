using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Specs;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using System.Reflection.Metadata;

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

        // Lấy danh sách PaperAccess
        var paperAccesses = paper.PaperAccesses.ToList();

        // Tạo một dictionary để lưu danh sách bài nộp theo lớp
        var classSubmissionsMap = new Dictionary<string, List<SubmitPaper>>();
        var submitPaperInClass = new List<SubmitPaper>();

        // Duyệt qua các lớp có trong PaperAccess
        foreach (var paperAccess in paperAccesses)
        {
            if (paperAccess.ClassId.HasValue)
            {
                var classroom = await _repoClass.FirstOrDefaultAsync(new ClassesByIdSpec(paperAccess.ClassId.Value), cancellationToken);
                if (classroom != null)
                {
                    // Lấy danh sách các bài nộp của lớp này
                    var studentIdsInClass = classroom.GetStudentIds();
                    var classSubmissions = await _repoSubmitPaper.ListAsync(new SubmitPaperByUserIdsSpec(request.PaperId, studentIdsInClass));
                    submitPaperInClass.AddRange(classSubmissions);
                    // Nếu lớp chưa tồn tại trong từ điển thì thêm vào
                    if (!classSubmissionsMap.ContainsKey(classroom.Name))
                    {
                        classSubmissionsMap[classroom.Name] = new List<SubmitPaper>();
                    }

                    // Thêm bài nộp vào danh sách
                    classSubmissionsMap[classroom.Name].AddRange(classSubmissions);
                }
            }
            else
            {
                // Xử lý trường hợp thí sinh tự do (không thuộc lớp nào)
                var submission = paper.SubmitPapers.FirstOrDefault(s => s.CreatedBy == paperAccess.UserId);
                if (submission != null)
                {
                    submitPaperInClass.Add(submission);
                    if (!classSubmissionsMap.ContainsKey(_t["Freelance contestant"]))
                    {
                        classSubmissionsMap[_t["Freelance contestant"]] = new List<SubmitPaper>();
                    }
                    classSubmissionsMap[_t["Freelance contestant"]].Add(submission);
                }
            }
        }

        // set freeblance who no t in paper access
        if (submitPaperInClass.Any())
        {
            if (!classSubmissionsMap.ContainsKey(_t["Freelance contestant"]))
            {
                classSubmissionsMap[_t["Freelance contestant"]] = new List<SubmitPaper>();
            }
            classSubmissionsMap[_t["Freelance contestant"]].AddRange(paper.SubmitPapers.Except(submitPaperInClass));
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

