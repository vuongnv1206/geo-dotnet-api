using FSH.WebApi.Application.Class.Specs;
using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents;
public class DeleteStudentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DeleteStudentRequest(Guid id)
    {
        Id = id;
    }
}

public class DeleteStudentRequestValidator : AbstractValidator<DeleteStudentRequest>
{
    public DeleteStudentRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteStudentRequestHandler : IRequestHandler<DeleteStudentRequest, Guid>
{
    private readonly IRepository<Student> _userStudentRepository;
    private readonly IStringLocalizer<DeleteStudentRequestHandler> _localizer;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;
    public DeleteStudentRequestHandler(
        IStringLocalizer<DeleteStudentRequestHandler> localizer,
        IRepository<Student> repository,
        ICurrentUser currentUser,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo,
        IRepository<Classes> classRepo)
    {
        _localizer = localizer;
        _userStudentRepository = repository;
        _currentUser = currentUser;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
        _classRepo = classRepo;
    }

    public async Task<Guid> Handle(DeleteStudentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var classOfStudent = await _classRepo.FirstOrDefaultAsync(new ClassByStudentClassIdSpec(request.Id));
        if (classOfStudent.CreatedBy != userId)
        {
            var groupPermissionSpec = new GroupPermissionClassByUserIdAndClassIdSpec(userId, classOfStudent.Id);
            var teacherPermissionSpec = new TeacherPermissionCLassByUserIdAndClassIdSpec(userId, classOfStudent.Id);

            var listPermission = new List<PermissionInClassDto>();

            listPermission.AddRange(await _groupPermissionRepo.ListAsync(groupPermissionSpec));
            listPermission.AddRange((await _teacherPermissionRepo
                                            .ListAsync(teacherPermissionSpec))
                                            .Where(x => !listPermission.Any(lp => lp.PermissionType == x.PermissionType)));

            if (!listPermission.Any(x => x.PermissionType == PermissionType.ManageStudentList))
                throw new NotFoundException(_localizer["Student in class {0} Not Found.", request.Id]);
        }

        var student = await _userStudentRepository.FirstOrDefaultAsync(new StudentByIdSpec(request.Id))
            ?? throw new NotFoundException(_localizer["Student in class {0} Not Found."]);


        await _userStudentRepository.DeleteAsync(student, cancellationToken);
        return student.Id;
    }
}   
