using FSH.WebApi.Application.Class.Specs;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.PaperStudents;
public class GetPendingTestOfStudentRequest
    : PaginationFilter, IRequest<PaginationResponse<StudentTestDto>>
{
    public CompletionStatusEnum CompletionStatus { get; set; }
}

public class GetPendingTestOfStudentRequestHandler
    : IRequestHandler<GetPendingTestOfStudentRequest, PaginationResponse<StudentTestDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IRepository<Paper> _paperRepo;

    public GetPendingTestOfStudentRequestHandler(
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IRepository<Paper> paperRepo)
    {
        _currentUser = currentUser;
        _classRepo = classRepo;
        _paperRepo = paperRepo;
    }

    public async Task<PaginationResponse<StudentTestDto>>
        Handle(GetPendingTestOfStudentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var joinedClasses = await _classRepo.ListAsync(new ClassesByStudentIdSpec(userId), cancellationToken);

        var joinedClassIds = joinedClasses.Select(x => x.Id).ToList();

        var studentIdsInclass = joinedClasses.SelectMany(x => x.UserClasses.Select(uc => uc.StudentId)).ToList();

        var spec = new PaperByIdsAndPublishSpec(request, joinedClassIds, studentIdsInclass, userId);

        var assignedPapers = await _paperRepo.ListAsync(spec, cancellationToken);
        var assignedPaperDtos = assignedPapers.Adapt<List<StudentTestDto>>();

        assignedPaperDtos.ForEach(x =>
        {
            x.CompletionStatus = request.CompletionStatus;
        });

        var res = new PaginationResponse<StudentTestDto>(assignedPaperDtos, assignedPapers.Count, request.PageNumber, request.PageSize);

        return res;
    }
}
