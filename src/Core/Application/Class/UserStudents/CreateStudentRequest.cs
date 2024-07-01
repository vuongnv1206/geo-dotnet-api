using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams.Specs;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents;
public class CreateStudentRequest : IRequest<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentCode { get; set; }
    public bool? Gender { get; set; }
    public Guid ClassesId { get; set; }
}

public class CreateUserStudentRequestHandler : IRequestHandler<CreateStudentRequest, Guid>
{
    private readonly IRepository<Student> _userStudentRepository;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<CreateUserStudentRequestHandler> _t;
    private readonly IRepository<Classes> _classRepository;

    public CreateUserStudentRequestHandler(
        IUserService userService,
        IStringLocalizer<CreateUserStudentRequestHandler> t,
        IRepository<Student> userStudentRepository,
        IRepository<Classes> classRepository)
    {
        _userService = userService;
        _t = t;
        _classRepository = classRepository;
        _userStudentRepository = userStudentRepository;
    }

    public async Task<DefaultIdType> Handle(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var userStudent = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            AvatarUrl = request.AvatarUrl,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
        };

        var existDuplicate = await _userStudentRepository.AnyAsync(new StudentByStudentCodeSpec(request.StudentCode));
        if (existDuplicate)
        {
            throw new ConflictException(_t["The student code '{0}' is already in use.", request.StudentCode]);
        }
        else
        {
            userStudent.StudentCode = request.StudentCode;
        }

        var userEmail = await _userService.GetUserDetailByEmailAsync(request.Email, cancellationToken);

        if (userEmail != null)
        {
            userStudent.Email = request.Email;
            userStudent.StId = userEmail.Id;
        }
        else
        {
            userStudent.Email = request.Email;
        }

        var userPhoneNumber = await _userService.GetUserDetailByPhoneAsync(request.PhoneNumber, cancellationToken);
        if (userPhoneNumber != null)
        {
            userStudent.PhoneNumber = request.PhoneNumber;
            userStudent.StId = userEmail.Id;
        }
        else
        {
            userStudent.PhoneNumber = request.PhoneNumber;
        }

        await _userStudentRepository.AddAsync(userStudent);
        await _userStudentRepository.SaveChangesAsync();


        var classes = await _classRepository.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassesId));
        if (classes == null)
        {
            throw new NotFoundException(_t["Class not found", request.ClassesId]);
        }

        var userClass = new UserClass
        {
            ClassesId = request.ClassesId,
            StudentId = userStudent.Id
        };

        classes.AddUserInClass(userClass);
        await _classRepository.UpdateAsync(classes);

        return userStudent.Id;
    }
}