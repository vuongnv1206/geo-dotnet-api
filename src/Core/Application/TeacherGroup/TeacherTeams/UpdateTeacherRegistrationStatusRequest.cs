using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class UpdateTeacherRegistrationStatusRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid? TeacherId { get; set; }
}

public class UpdateTeacherRegistrationStatusRequestHandler : IRequestHandler<UpdateTeacherRegistrationStatusRequest, Guid>
{
    private readonly IRepositoryWithEvents<TeacherTeam> _repository;
    private readonly IStringLocalizer _t;

    public UpdateTeacherRegistrationStatusRequestHandler(IRepositoryWithEvents<TeacherTeam> repository, IStringLocalizer<UpdateTeacherRegistrationStatusRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);
    public async Task<Guid> Handle(UpdateTeacherRegistrationStatusRequest request, CancellationToken cancellationToken)
    {
        var teacherTeam = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = teacherTeam
        ?? throw new NotFoundException(_t["TeacherTeam {0} Not Found.", request.Id]);

        teacherTeam.UpdateRegistrationStatus(request.TeacherId);

        await _repository.UpdateAsync(teacherTeam, cancellationToken);

        return request.Id;
    }
}
