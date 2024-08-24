using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class GetSubmittedPaperRequest : PaginationFilter, IRequest<PaginationResponse<SubmitPaperDto>>
{
    public Guid PaperId { get; set; }
    public Guid? ClassId { get; set; }
}

public class GetSubmittedPaperRequestHandler : IRequestHandler<GetSubmittedPaperRequest, PaginationResponse<SubmitPaperDto>>
{
    private readonly IReadRepository<SubmitPaper> _repository;
    private readonly IRepository<Paper> _paperRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;
    private readonly IRepository<Classes> _classRepo;

    public GetSubmittedPaperRequestHandler(
        IReadRepository<SubmitPaper> repository,
        ICurrentUser currentUser,
        IStringLocalizer<GetSubmittedPaperRequestHandler> t,
        IRepository<Paper> paperRepo,
        IUserService userService,
        IRepository<Classes> classRepo)
    {
        _repository = repository;
        _currentUser = currentUser;
        _t = t;
        _paperRepo = paperRepo;
        _userService = userService;
        _classRepo = classRepo;
    }

    public async Task<PaginationResponse<SubmitPaperDto>> Handle(GetSubmittedPaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _paperRepo.FirstOrDefaultAsync(new PaperByIdSpec(request.PaperId), cancellationToken)
            ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.PaperId]);

        var currentUserId = _currentUser.GetUserId();
        var studentIds = new List<Guid?>();
        if (request.ClassId.HasValue)
        {
            var classroom = await _classRepo.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassId.Value), cancellationToken)
                ?? throw new NotFoundException(_t["Class {0} Not Found.", request.ClassId]);

            // lấy ra stId của studnet trong class
            studentIds = classroom.UserClasses.Where(x => x.Student.StId != null).Select(x => x.Student.StId).ToList();
        }

        bool isTeacher = false;

        if (paper.CreatedBy == currentUserId
            || paper.PaperPermissions.Any(x => x.UserId.HasValue && x.UserId.Value == currentUserId && x.CanView)
            || paper.PaperPermissions.Any(x => x.GroupTeacherId.HasValue && x.GroupTeacher.TeacherInGroups.Any(tig => tig.TeacherTeam.TeacherId == currentUserId) && x.CanView))
        {
            isTeacher = true;
        }

        var spec = new SubmitPaperByPaperIdPaging(request, paper, currentUserId, studentIds, isTeacher);
        var submitPapers = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
        foreach (var submitter in submitPapers.Data)
        {
            submitter.CreatorName = await _userService.GetFullName(submitter.CreatedBy);
        }

        return submitPapers;
    }
}


