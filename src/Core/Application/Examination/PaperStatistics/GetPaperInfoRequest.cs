using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class GetPaperInfoRequest : IRequest<PaperInfoStatistic>
{
    public Guid PaperId { get; set; }
    public Guid? ClassId { get; set; }
    public GetPaperInfoRequest(Guid paperId, DefaultIdType? classId)
    {
        PaperId = paperId;
        ClassId = classId;
    }
}

public class GetPaperInfoRequestHandler : IRequestHandler<GetPaperInfoRequest, PaperInfoStatistic>
{
    private readonly IRepository<Paper> _paperRepo;
    private readonly IRepository<Classes> _classRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;

    public GetPaperInfoRequestHandler(
        IRepository<Paper> paperRepo,
        ICurrentUser currentUser,
        IStringLocalizer<GetPaperInfoRequestHandler> t,
        IRepository<Classes> classRepo)
    {
        _paperRepo = paperRepo;
        _currentUser = currentUser;
        _t = t;
        _classRepo = classRepo;
    }

    public async Task<PaperInfoStatistic> Handle(GetPaperInfoRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId))
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);
        if (request.ClassId.HasValue)
        {
            var classRoom = await _classRepo.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassId.Value, userId));
            paper.SubmitPapers = paper.SubmitPapers.Where(x => classRoom.UserClasses.Any(uc => uc.Student.StId == x.CreatedBy)).ToList();
        }

        var response = paper.Adapt<PaperInfoStatistic>();

        if (paper.PaperAccesses.Any())
        {
            response.TotalRegister = 0;
            var studentsInclass = new List<UserClass>();
            foreach (var paperAccess in paper.PaperAccesses
                .Where(x => !request.ClassId.HasValue || x.ClassId == request.ClassId))
            {
                if (paperAccess.ClassId.HasValue)
                {
                    var classRoom = await _classRepo.FirstOrDefaultAsync(new ClassByIdSpec(paperAccess.ClassId.Value, userId));
                    if (classRoom.UserClasses.Any())
                    {
                        studentsInclass.AddRange(classRoom.UserClasses);
                    }
                }
            }

            foreach (var paperAccess in paper.PaperAccesses.Where(x => x.UserId.HasValue))
            {
                if (studentsInclass.Any(x => x.StudentId != paperAccess.UserId))
                {
                    response.TotalRegister++;
                }
            }

            response.TotalRegister += studentsInclass.GroupBy(x => x.Student.Email).Count();
            response.TotalNotComplete = response.TotalRegister - response.TotalAttendee;
        }

        return response;
    }
}
