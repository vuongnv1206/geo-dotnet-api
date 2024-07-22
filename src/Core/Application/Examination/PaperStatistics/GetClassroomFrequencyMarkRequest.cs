using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.Matrices;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetClassroomFrequencyMarkRequest : IRequest<ClassroomFrequencyMarkDto>
{
    public Guid PaperId { get; set; }
    public Guid? ClassroomId { get; set; }
}

public class GetClassroomFrequencyMarkRequestHandler : IRequestHandler<GetClassroomFrequencyMarkRequest, ClassroomFrequencyMarkDto>
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

    public async Task<ClassroomFrequencyMarkDto> Handle(GetClassroomFrequencyMarkRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repoPaper.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);
        var currentUserId = _currentUser.GetUserId();
        List<SubmitPaper> submissions = new List<SubmitPaper>();
        
        var accessibleStudentIds = new List<Guid>();
        var classroom = new Classes();
        //Filter theo classroom
        if (request.ClassroomId.HasValue)
        {
            classroom = await _repoClass.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassroomId.Value, currentUserId));
            var studentIdsInClass = classroom.GetStudentIds();
            accessibleStudentIds.AddRange(studentIdsInClass);
            submissions.AddRange(await _repoSubmitPaper.ListAsync(new SubmitPaperByUserIdsSpec(request.PaperId, accessibleStudentIds), cancellationToken));
        }
        else //Lấy tất
        {
            submissions.AddRange(paper.SubmitPapers);
            //var accessibleClassIds = paper.PaperAccesses.Select(x => x.ClassId).ToList();
            //if (accessibleClassIds.Any())
            //{
            //    foreach (var classId in accessibleClassIds)
            //    {
            //        var classroomAccessible = await _repoClass.FirstOrDefaultAsync(new ClassByIdSpec(classId.Value, currentUserId));
            //        var studentIdsInClass = classroomAccessible.GetStudentIds();
            //        accessibleStudentIds.AddRange(studentIdsInClass);
            //    }
            //}
            //accessibleStudentIds.AddRange(paper.PaperAccesses.Select(x => x.UserId.Value));
            //submissions.AddRange(await _repoSubmitPaper.ListAsync(new SubmitPaperByUserIdsSpec(request.PaperId, accessibleStudentIds), cancellationToken));
        }

        var totalRegister = 0;
        var totalAttendee = 0;

        if (paper.ShareType != Domain.Examination.Enums.PaperShareType.All && paper.PaperAccesses.Any())
        {
             totalRegister = submissions.Count;
             totalAttendee = submissions.Where(x => x.Status == SubmitPaperStatus.End).Count();
        }
        else
        {
             totalRegister = paper.SubmitPapers.Count();
             totalAttendee = paper.SubmitPapers.Where(x => x.Status == SubmitPaperStatus.End).Count();
        }

        var maxPointInPaper = paper.PaperQuestions.Sum(x => x.Mark);

        // Chia thang điểm thành 10 phần dựa trên maxPointInPaper
        var interval = maxPointInPaper / 10.0;
        var frequencyMarks = new List<FrequencyMarkDto>();
        for (int i = 0; i < 10; i++)
        {
            var fromMark = i * interval;
            var toMark = (i + 1) * interval;

            frequencyMarks.Add(new FrequencyMarkDto
            {
                FromMark = (float)fromMark,
                ToMark = (float)toMark,
                Total = submissions.Count(s => s.TotalMark >= fromMark && s.TotalMark <= toMark)
            });
        }


        return new ClassroomFrequencyMarkDto
        {
            Class =  classroom.Adapt<ClassDto>(),
            TotalRegister = totalRegister,
            TotalAttendee = totalAttendee,
            FrequencyMarks = frequencyMarks
        };


    }
}
