﻿using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.TeacherTeams;
public class GetTeacherTeamRequest : IRequest<TeacherTeamDto>
{
    public Guid Id { get; set; }
    public GetTeacherTeamRequest(Guid id)
    {
        Id = id;
    }
}

public class GetTeacherTeamRequestHandler : IRequestHandler<GetTeacherTeamRequest, TeacherTeamDto>
{
    private readonly IRepository<TeacherTeam> _repository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public GetTeacherTeamRequestHandler(
        IRepository<TeacherTeam> repository,
        IStringLocalizer<GetTeacherTeamRequestHandler> t,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _t = t;
        _currentUser = currentUser;
    }

    public async Task<TeacherTeamDto> Handle(GetTeacherTeamRequest request, CancellationToken cancellationToken)
    {
        var data = await _repository.FirstOrDefaultAsync(
            (ISpecification<TeacherTeam, TeacherTeamDto>)new TeacherTeamByIdSpec(request.Id, _currentUser.GetUserId()), cancellationToken);

        if (data == null)
            throw new NotFoundException(_t["TeacherTeam{0} Not Found.", request.Id]);

        return data;
    }
}
