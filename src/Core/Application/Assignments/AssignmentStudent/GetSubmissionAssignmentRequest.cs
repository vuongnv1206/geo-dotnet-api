﻿using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Shared.Authorization;
using Mapster;

namespace FSH.WebApi.Application.Assignments.AssignmentStudent;
public class GetSubmissionAssignmentRequest : IRequest<List<SubmissionAssignmentDto>>
{
    public Guid AssignmentId { get; set; }
    public Guid? ClassId { get; set; }
}

public class GetSubmissionAssignmentRequestHandler : IRequestHandler<GetSubmissionAssignmentRequest, List<SubmissionAssignmentDto>>
{
    private readonly IRepository<Assignment> _repositoryAssignment;
    private readonly IStringLocalizer<GetSubmissionAssignmentRequestHandler> _t;
    private readonly IUserService _userService;
    private readonly IRepository<Classes> _classesRepository;
    private readonly ICurrentUser _currentUser;

    public GetSubmissionAssignmentRequestHandler(IRepository<Assignment> repositoryAssignment, IStringLocalizer<GetSubmissionAssignmentRequestHandler> t, IUserService userService, IRepository<Classes> classesRepository, ICurrentUser currentUser)
    {
        _repositoryAssignment = repositoryAssignment;
        _t = t;
        _userService = userService;
        _classesRepository = classesRepository;
        _currentUser = currentUser;
    }

    public async Task<List<SubmissionAssignmentDto>> Handle(GetSubmissionAssignmentRequest request, CancellationToken cancellationToken)
    {
        var assignment = await _repositoryAssignment.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId));
        _ = assignment ?? throw new NotFoundException(_t["Assignment {0} Not  Found.", request.AssignmentId]);

        var submissionAssignmentDto = assignment.AssignmentStudents.Adapt<List<SubmissionAssignmentDto>>();
        var currenrUserId = _currentUser.GetUserId();
        if (_currentUser.IsInRole(FSHRoles.Student))
        {
            if (request.ClassId.HasValue)
            {
                var classroom = await _classesRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassId.Value));
                _ = classroom ?? throw new NotFoundException(_t["Class {0} Not Found.", request.ClassId]);

                var studentIds = classroom.UserClasses?.Select(x => x.StudentId).ToList();

                submissionAssignmentDto = submissionAssignmentDto
                .Where(submission => studentIds.Contains(submission.StudentId) && submission.CreatedBy == currenrUserId)
                .ToList();
            }
        }
        else //Teacher or Manager
        {
            if (request.ClassId.HasValue)
            {
                var classroom = await _classesRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassId.Value));
                _ = classroom ?? throw new NotFoundException(_t["Class {0} Not Found.", request.ClassId]);

                var studentIds = classroom.UserClasses?.Select(x => x.StudentId).ToList();

                submissionAssignmentDto = submissionAssignmentDto
                .Where(submission => studentIds.Contains(submission.StudentId))
                .ToList();
            }
        }

       

        return submissionAssignmentDto;
    }
}