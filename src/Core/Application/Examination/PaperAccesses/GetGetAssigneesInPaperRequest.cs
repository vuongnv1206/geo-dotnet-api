using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperAccesses;
public class GetGetAssigneesInPaperRequest : PaginationFilter, IRequest<PaginationResponse<ClassAccessPaper>>
{
    public Guid PaperId { get; set; }
    public Guid? GroupClassId { get; set; }
    public Guid? ClassId { get; set; }
}

public class GetGetAssigneesInPaperRequestHandler : IRequestHandler<GetGetAssigneesInPaperRequest, PaginationResponse<ClassAccessPaper>>
{

    private readonly IRepository<Paper> _paperRepo;
    private readonly IStringLocalizer _t;
    private readonly IRepository<Classes> _classRepo;

    public GetGetAssigneesInPaperRequestHandler(
        IRepository<Paper> paperRepo,
        IStringLocalizer<GetGetAssigneesInPaperRequestHandler> t,
        IRepository<Classes> classRepo)
    {
        _paperRepo = paperRepo;
        _t = t;
        _classRepo = classRepo;
    }

    public async Task<PaginationResponse<ClassAccessPaper>> Handle(GetGetAssigneesInPaperRequest request, CancellationToken cancellationToken)
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
                if (access.Class.UserClasses != null)
                {
                    accessStudentIds.AddRange(access.Class.UserClasses.Select(x => x.StudentId));
                }
            }
            else if (access.UserId != null)
            {
                accessStudentIds.Add(access.UserId.Value);
            }
        }

        var spec = new ClassHasStudentAccessPaperSpec(request, accessStudentIds, paper.CreatedBy);

        var classrooms = await _classRepo.ListAsync(spec, cancellationToken);

        var classMap = classrooms.Adapt<List<ClassAccessPaper>>();

        return new PaginationResponse<ClassAccessPaper>(classMap, classrooms.Count, request.PageNumber, request.PageSize);
    }
}
