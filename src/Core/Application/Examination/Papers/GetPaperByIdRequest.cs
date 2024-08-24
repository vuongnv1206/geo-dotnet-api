using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Application.Examination.Papers;
public class GetPaperByIdRequest : IRequest<PaperDto>
{
    public Guid Id { get; set; }
    public GetPaperByIdRequest(Guid id)
    {
        Id = id;
    }
}

public class GetPaperByIdRequestHandler : IRequestHandler<GetPaperByIdRequest, PaperDto>
{
    private readonly IRepository<Paper> _repository;
    private readonly IStringLocalizer _t;
    private readonly IUserService _userService;
    private readonly IRepository<GroupTeacher> _groupTeacherRepo;
    public GetPaperByIdRequestHandler(IRepository<Paper> repository, IStringLocalizer<GetPaperByIdRequestHandler> t, IUserService userService, IRepository<GroupTeacher> groupTeacherRepo)
    {
        _repository = repository;
        _t = t;
        _userService = userService;
        _groupTeacherRepo = groupTeacherRepo;
    }

    public async Task<PaperDto> Handle(GetPaperByIdRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperByIdSpec(request.Id);
        var paper = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        var paperDto = paper.Adapt<PaperDto>();
        paperDto.CreatorName = await _userService.GetFullName(paper.CreatedBy);
        paperDto.TotalAttended = paper.GetTotalSubmissions();

        if (paperDto.PaperPermissions.Any())
        {
            foreach (var per in paperDto.PaperPermissions)
            {
                if (per.UserId.HasValue)
                {
                    var user_permission = await _userService.GetAsync(per.UserId.ToString(), cancellationToken);
                    if (user_permission != null)
                    {
                        per.User = user_permission;
                    }
                }
                if (per.GroupTeacherId.HasValue)
                {
                    var teacherGroup = await _groupTeacherRepo.FirstOrDefaultAsync(new GroupTeacherByIdSpec(per.GroupTeacherId.Value), cancellationToken);
                    if (teacherGroup != null)
                    {
                        per.GroupTeacher = teacherGroup.Adapt<GroupTeacherDto>();
                    }
                }
            }
        }


        return paperDto;

    }
}
