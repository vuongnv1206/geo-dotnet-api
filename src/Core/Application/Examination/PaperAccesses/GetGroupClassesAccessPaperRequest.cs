using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperAccesses;
public class GetGroupClassesAccessPaperRequest : PaginationFilter, IRequest<PaginationResponse<GroupClassAccessPaper>>
{
    public Guid PaperId { get; set; }
    public PaperShareType Status { get; set; }

    public GetGroupClassesAccessPaperRequest(DefaultIdType paperId)
    {
        PaperId = paperId;
    }
}

public class GetGroupClassesAccessPaperRequestHandler : IRequestHandler<GetGroupClassesAccessPaperRequest, PaginationResponse<GroupClassAccessPaper>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Paper> _paperRepo;
    private readonly IStringLocalizer _t;
    private readonly IRepository<GroupClass> _groupRepo;

    public GetGroupClassesAccessPaperRequestHandler(
        ICurrentUser currentUser,
        IRepository<Paper> paperRepo,
        IStringLocalizer<GetGroupClassesAccessPaperRequestHandler> t,
        IRepository<GroupClass> groupRepo)
    {
        _currentUser = currentUser;
        _paperRepo = paperRepo;
        _t = t;
        _groupRepo = groupRepo;
    }

    public async Task<PaginationResponse<GroupClassAccessPaper>> Handle(GetGroupClassesAccessPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId))
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var accessGroupClasses = new List<GroupClassAccessPaper>();
        var accessClassIds = new List<Guid>();
        var accessStudentIds = new List<Guid>();
        foreach (var access in paper.PaperAccesses)
        {
            if (access.ClassId != null)
            {
                accessClassIds.Add(access.ClassId.Value);
            }
            else if (access.UserId != null)
            {
                accessStudentIds.Add(access.UserId.Value);
            }
        }

        GroupClassAccessPaperSpec spec;
            
        if (request.Status == PaperShareType.AssignToStudent)
        {
            spec = new GroupClassAccessPaperSpec(accessStudentIds, request, paper.CreatedBy, request.Status);
        }else if (request.Status == PaperShareType.AssignToClass)
        {
            spec = new GroupClassAccessPaperSpec(accessClassIds, request, paper.CreatedBy, request.Status);
        }else
        {
            spec = new GroupClassAccessPaperSpec(accessClassIds, accessStudentIds, request, paper.CreatedBy);
        }

        var groups = await _groupRepo.ListAsync(spec, cancellationToken);

        foreach (var group in groups)
        {
            group.Classes = group.Classes.Where(c => accessClassIds.Contains(c.Id) || c.UserClasses.Any(uc => accessStudentIds.Contains(uc.StudentId))).ToList();

            foreach(var classroom in group.Classes)
            {
                classroom.UserClasses = classroom.UserClasses.Where(uc => accessStudentIds.Contains(uc.StudentId)).ToList();
            }
        }

        var data = groups.Adapt<List<GroupClassAccessPaper>>();
        return new PaginationResponse<GroupClassAccessPaper>(data, groups.Count, request.PageNumber, request.PageSize);
    }
}
