using FSH.WebApi.Application.Class;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Assignments.AssignmentClasses;

public class RemoveAssignmentFromClassRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public Guid ClassId { get; set; }
}

public class RemoveAssignmentFromClassRequestValidator : CustomValidator<RemoveAssignmentFromClassRequest>
{
    public RemoveAssignmentFromClassRequestValidator()
    {

    }
}

public class RemoveAssignmentFromClassRequestHandler : IRequestHandler<RemoveAssignmentFromClassRequest, Guid>
{
    private readonly IRepository<Classes> _classesRepository;
    private readonly IStringLocalizer _t;
    private readonly ICurrentUser _currentUser;

    public RemoveAssignmentFromClassRequestHandler(
        IStringLocalizer<RemoveAssignmentFromClassRequestHandler> t,
        IRepository<Classes> classesRepository,
        ICurrentUser currentUser)
    {
        _t = t;
        _classesRepository = classesRepository;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(RemoveAssignmentFromClassRequest request, CancellationToken cancellationToken)
    {
        var classroom = await _classesRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassId));

        _ = classroom ?? throw new NotFoundException(_t["Class {0} Not Found.", request.ClassId]);

        classroom.RemoveAssignment(request.AssignmentId);

        await _classesRepository.UpdateAsync(classroom, cancellationToken);

        return classroom.Id;
    }
}