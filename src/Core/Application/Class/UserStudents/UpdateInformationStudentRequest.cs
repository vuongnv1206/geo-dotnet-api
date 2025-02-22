﻿using FSH.WebApi.Application.Class.Specs;
using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.Class.UserStudents;
public class UpdateInformationStudentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentCode { get; set; }
    public bool? Gender { get; set; }
}

public class UpdateInformationStudentRequestHandler : IRequestHandler<UpdateInformationStudentRequest, Guid>
{
    private readonly IRepositoryWithEvents<Student> _repository;
    private readonly IStringLocalizer _t;
    private readonly IRepository<Classes> _classRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;
    private readonly IRepository<Student> _userStudentRepository;
    public UpdateInformationStudentRequestHandler(
        IRepositoryWithEvents<Student> repository,
        IStringLocalizer<UpdateInformationStudentRequestHandler> t,
        IRepository<Classes> classRepository,
        ICurrentUser currentUser,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo,
        IRepository<Student> userStudentRepository)
    {
        _repository = repository;
        _t = t;
        _classRepository = classRepository;
        _currentUser = currentUser;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
        _userStudentRepository = userStudentRepository;
    }

    public async Task<Guid> Handle(UpdateInformationStudentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var classOfStudent = await _classRepository.FirstOrDefaultAsync(new ClassByStudentClassIdSpec(request.Id));
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
                throw new ForbiddenException(_t["You don't have student management"]);
        }

        var student = await _repository.FirstOrDefaultAsync(new StudentByIdSpec(request.Id), cancellationToken);
        _ = student ?? throw new NotFoundException(_t["Student {0} Not Found.", request.Id]);

        // if (request.Email is not null && !student.IsValidEmail(request.Email))
        //    throw new ConflictException(_t["The email address '{0}' is not valid.", request.Email]);
        // if (request.PhoneNumber is not null && !student.IsValidPhoneNumber(request.PhoneNumber))
        //    throw new ConflictException(_t["The phone number '{0}' is not valid. It must be 10 digits.", request.PhoneNumber]);

        if (classOfStudent.UserClasses != null)
        {
            if (classOfStudent.UserClasses.Any(x => x.Student.Email.Trim() == request.Email.Trim() && x.Student.Id != request.Id))
                throw new BadRequestException(_t["Email is existed in class"]);
            else if (classOfStudent.UserClasses.Any(x => x.Student.PhoneNumber.Trim() == request.PhoneNumber.Trim() && x.Student.Id != request.Id))
                throw new BadRequestException(_t["Phone number is existed in class"]);
            else if (classOfStudent.UserClasses.Any(x => x.Student.StudentCode.Trim() == request.StudentCode.Trim() && x.Student.Id != request.Id))
                throw new BadRequestException(_t["Student code is existed in class"]);
        }

        var updatedStudent = student.Update(
            request.FirstName,
            request.LastName,
            request.AvatarUrl,
            request.DateOfBirth,
            request.Email,
            request.PhoneNumber,
            request.StudentCode,
            request.Gender);

        await _repository.UpdateAsync(updatedStudent, cancellationToken);
        return request.Id;
    }
}
