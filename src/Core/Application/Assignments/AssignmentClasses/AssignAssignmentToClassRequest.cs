using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Assignments.AssignmentClasses;

public class AssignAssignmentToClassRequest : IRequest<Guid>
{
    public Guid AssignmentId { get; set; }
    public Guid ClassesdId { get; set; }
}

public class AssignAssignmentToClassRequestValidator : CustomValidator<AssignAssignmentToClassRequest>
{
    public AssignAssignmentToClassRequestValidator()
    {

    }
}

//public class AssignAssignmentToClassRequestHandler : IRequestHandler<AssignAssignmentToClassRequest, Guid>
//{
//    //private readonly IRepository<Classes> _classesRepository;
//    //private readonly IUserService _userService;
//    //private readonly IStringLocalizer _t;
//    //private readonly ICurrentUser _currentUser;
//    //private readonly IReadRepository<Assignment> _assignmentRepository;

//    //public AssignAssignmentToClassRequestHandler(
//    //    IRepository<Classes> classesRepository,
//    //    IUserService userService,
//    //    IStringLocalizer<AssignAssignmentToClassRequestHandler> t,
//    //    ICurrentUser currentUser,
//    //    IReadRepository<Assignment> assignmentRepository)
//    //    => (_classesRepository, _userService, _t, _currentUser, _assignmentRepository)
//    //        = (classesRepository, userService, t, currentUser, assignmentRepository);

//    //public async Task<DefaultIdType> Handle(AssignAssignmentToClassRequest request, CancellationToken cancellationToken)
//    //{
//    //    var classes = await _classesRepository.GetByIdAsync(request.ClassesdId);
//    //    if (classes is null)
//    //        throw new NotFoundException(_t["Class {0} Not Found.", request.ClassesdId]);

//    //    var userId = _currentUser.GetUserId();
//    //    if (!classes.CanUpdate(_currentUser.GetUserId()))
//    //        throw new ForbiddenException(_t["You cannot have permission update with {0}", classes.Name]);

//    //    var assignmentInClass = await _assignmentRepository.FirstOrDefaultAsync(new AssignmentByIdSpec(request.AssignmentId, userId));

//    //    if (assignmentInClass is null)
//    //    {
//    //        throw new NotFoundException(_t["Assignment {0} Not Found.", request.AssignmentId]);
//    //    }
//    //    else
//    //    {
//    //        if (assignmentInClass.Id == Guid.Empty)
//    //            throw new NotFoundException(_t["Assignment {0} Not Found.", request.AssignmentId]);

//    //        classes.AssignAssignmentToClass(new AssignmentClass
//    //        {
//    //            AssignmentId = request.AssignmentId,
//    //            ClassesId = request.ClassesdId
//    //        });

//    //        await _classesRepository.UpdateAsync(classes);

//    //    }

//    //    return default(DefaultIdType);
//    //}
//}
