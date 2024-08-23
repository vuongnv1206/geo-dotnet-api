using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Assignments.AssignmentClasses;
internal class GetPapersInClassRequest : IRequest<List<PaperInListDto>>
{
    public Guid ClassId { get; set; }

    public GetPapersInClassRequest(Guid classId)
    {
        ClassId = classId;
    }
}

internal class GetPapersInClassRequestHandler : IRequestHandler<GetPapersInClassRequest, List<PaperInListDto>>
{
    private readonly IRepository<Classes> _classesRepository;
    private readonly IStringLocalizer<GetPapersInClassRequestHandler> _t;
    private readonly IUserService _userService;
    private readonly IRepository<Paper> _paperRepository;

    public GetPapersInClassRequestHandler(IRepository<Classes> classesRepository, IStringLocalizer<GetPapersInClassRequestHandler> t, IUserService userService, IRepository<Paper> paperRepository)
    {
        _classesRepository = classesRepository;
        _t = t;
        _userService = userService;
        _paperRepository = paperRepository;
    }

    public async Task<List<PaperInListDto>> Handle(GetPapersInClassRequest request, CancellationToken cancellationToken)
    {
        var classroom = await _classesRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassId));
        _ = classroom ?? throw new NotFoundException(_t["Class {0} Not Found.", request.ClassId]);

        var papersInClass = await _paperRepository.ListAsync(new PapersByClassIdSpec(request.ClassId), cancellationToken);

        // Chuyển đổi các Paper thành DTOs
        var paperDtos = papersInClass.Adapt<List<PaperInListDto>>();

        return paperDtos;
    }
}
