using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using Mapster;

namespace FSH.WebApi.Application.Assignments.AssignmentClasses;
public class GetAssignmentInClassRequest : IRequest<List<AssignmentDto>>
{
    public Guid ClassId { get; set; }
    public GetAssignmentInClassRequest(Guid classId)
    {
        ClassId = classId;
    }
}

public class GetAssignmentInClassRequestHandler : IRequestHandler<GetAssignmentInClassRequest, List<AssignmentDto>>
{
    private readonly IRepository<Classes> _classesRepository;
    private readonly IStringLocalizer<GetAssignmentInClassRequestHandler> _t;
    private readonly IUserService _userService;
    private readonly IRepository<Assignment> _assignmentRepository;

    public GetAssignmentInClassRequestHandler(IRepository<Classes> classesRepository, IStringLocalizer<GetAssignmentInClassRequestHandler> t, IUserService userService, IRepository<Assignment> assignmentRepository)
    {
        _classesRepository = classesRepository;
        _t = t;
        _userService = userService;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<List<AssignmentDto>> Handle(GetAssignmentInClassRequest request, CancellationToken cancellationToken)
    {

        var classroom = await _classesRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassId));
        _ = classroom ?? throw new NotFoundException(_t["Class {0} Not Found.", request.ClassId]);

        var assignments = classroom.AssignmentClasses?.Select(x => x.Assignment).ToList();
        var assignmentDtos = assignments.Adapt<List<AssignmentDto>>();

        return assignmentDtos;
    }
}


