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
public class CreateUserStudentRequest : IRequest<DefaultIdType>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentCode { get; set; }
    public bool? Gender { get; set; }
    public Guid ClassesId { get; set; }
}

public class CreateUserStudentRequestHandler : IRequestHandler<CreateUserStudentRequest, DefaultIdType>
{
    private readonly IRepository<UserStudent> _userStudentRepository;
    private readonly IRepositoryWithEvents<UserStudent> _userStudentEventRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<CreateUserStudentRequestHandler> _t;
    private readonly IRepository<Classes> _classRepository;

    public CreateUserStudentRequestHandler(IRepositoryWithEvents<UserStudent> userStudentEventRepository, IUserService userService, ICurrentUser currentUser,
                                           IStringLocalizer<CreateUserStudentRequestHandler> t,IRepository<UserStudent> userStudentRepository, IRepository<Classes> classRepository)
    {
        _userStudentEventRepository = userStudentEventRepository;
        _userService = userService;
        _currentUser = currentUser;
        _t = t;
        _classRepository = classRepository;
        _userStudentRepository = userStudentRepository;
    }
    public async Task<DefaultIdType> Handle(CreateUserStudentRequest request, CancellationToken cancellationToken)
    {
        var existDuplicate = await _userStudentRepository.AnyAsync(new UserStudentByInformationSpec(request.Email, request.PhoneNumber, _currentUser.GetUserId()), cancellationToken);

        if (existDuplicate)
        {
            throw new ConflictException(_t["User information exist in class"]);
        }

        var userStudent = new UserStudent
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            StudentCode = request.StudentCode,
            Gender = request.Gender,
        };

        var userEmail = await _userService.GetUserDetailByEmailAsync(request.Email, cancellationToken);

        if (userEmail != null)
        {
            userStudent.Email = request.Email;
            userStudent.StudentId = userEmail.Id;
        }

        var userPhoneNumber = await _userService.GetUserDetailByPhoneAsync(request.PhoneNumber, cancellationToken);
        if (userPhoneNumber != null)
        {
            userStudent.PhoneNumber = request.PhoneNumber;
            userStudent.StudentId = userEmail.Id;
        }

        await _userStudentRepository.AddAsync(userStudent);
        await _userStudentRepository.SaveChangesAsync();


        var classes = await _classRepository.GetByIdAsync(request.ClassesId, cancellationToken);
        if(classes == null)
        {
            throw new NotFoundException(_t["Class not found"]);
        }

        var userClass = new UserClass
        {
            ClassesId = request.ClassesId,
            UserStudentId = userStudent.Id
        };

        classes.AddUserInClass(userClass);
        await _classRepository.SaveChangesAsync();
        return userStudent.Id;
    }
}