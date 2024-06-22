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
    public Guid ClassesdId { get; set; }
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
        var classes = await _classesRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassesdId));
        //var classes = await _classesRepository.GetByIdAsync(request.ClassesdId);
        if (classes is null)
            throw new NotFoundException(_t["Class {0} Not Found.", request.ClassesdId]);

        if (!classes.CanUpdate(_currentUser.GetUserId()))
            throw new ForbiddenException(_t["You cannot have permission update with {0}", request.ClassesdId]);

        if (classes.AssignmentClasses.Any())
        {
            var assignmentInClass = classes.AssignmentClasses?.FirstOrDefault(x => x.AssignmentId == request.AssignmentId && x.ClassesId == request.ClassesdId);

            if (assignmentInClass is null)
            {
                throw new NotFoundException(_t["Assignment {0} Not Found.", request.AssignmentId]);
            }

            classes.RemoveAssignmentFromClass(assignmentInClass);

            await _classesRepository.UpdateAsync(classes);
        }

        return default(DefaultIdType);
    }
}