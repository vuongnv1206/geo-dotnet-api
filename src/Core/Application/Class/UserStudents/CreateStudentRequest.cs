using FSH.WebApi.Application.Assignments.AssignmentStudent;
using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System.Text.RegularExpressions;

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
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<GroupPermissionInClass> _groupPermissionRepo;
    private readonly IRepository<TeacherPermissionInClass> _teacherPermissionRepo;
    private readonly IMediator _mediator;

    public CreateUserStudentRequestHandler(
        IUserService userService,
        IStringLocalizer<CreateUserStudentRequestHandler> t,
        IRepository<Student> userStudentRepository,
        IRepository<Classes> classRepository,
        ICurrentUser currentUser,
        IRepository<GroupPermissionInClass> groupPermissionRepo,
        IRepository<TeacherPermissionInClass> teacherPermissionRepo,
        IMediator mediator)
    {
        _userService = userService;
        _t = t;
        _classRepository = classRepository;
        _userStudentRepository = userStudentRepository;
        _currentUser = currentUser;
        _groupPermissionRepo = groupPermissionRepo;
        _teacherPermissionRepo = teacherPermissionRepo;
        _mediator = mediator;
    }

    public async Task<DefaultIdType> Handle(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();
        var classroom = await _classRepository.FirstOrDefaultAsync(new ClassByIdSpec(request.ClassesId, userId))
            ?? throw new NotFoundException(_t["Class not found", request.ClassesId]);

        if (classroom.CreatedBy != userId)
        {
            var groupPermissionSpec = new GroupPermissionClassByUserIdAndClassIdSpec(userId, request.ClassesId);
            var teacherPermissionSpec = new TeacherPermissionCLassByUserIdAndClassIdSpec(userId, request.ClassesId);

            var listPermission = new List<PermissionInClassDto>();

            listPermission.AddRange(await _groupPermissionRepo.ListAsync(groupPermissionSpec));
            listPermission.AddRange((await _teacherPermissionRepo
                                            .ListAsync(teacherPermissionSpec))
                                            .Where(x => !listPermission.Any(lp => lp.PermissionType == x.PermissionType)));

            if (!listPermission.Any(x => x.PermissionType == PermissionType.ManageStudentList))
                throw new NotFoundException(_t["Classes {0} Not Found.", request.ClassesId]);
        }

        if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.PhoneNumber))
        {
            throw new BadRequestException(_t["An email address or phone number is required"]);
        }

        if (string.IsNullOrEmpty(request.StudentCode))
        {
            throw new BadRequestException(_t["a studetn code is required"]);
        }


        if (classroom.UserClasses != null)
        {
            if (classroom.UserClasses.Any(x => x.Student.Email.Trim() == request.Email.Trim()))
                throw new BadRequestException(_t["Email is existed in class"]);
            else if (classroom.UserClasses.Any(x => x.Student.PhoneNumber.Trim() == request.PhoneNumber.Trim()))
                throw new BadRequestException(_t["Phone number is existed in class"]);
            else if (classroom.UserClasses.Any(x => x.Student.StudentCode.Trim() == request.StudentCode.Trim()))
                throw new BadRequestException(_t["Code student is existed in class"]);
        }

        if (request.DateOfBirth.HasValue)
        {
            var today = DateTime.Today;

            int age = today.Year - request.DateOfBirth.Value.Year;

            if (age < 12)
            {
                throw new BadRequestException(_t["The student is not yet 12 years old"]);
            }
        }

        if (!string.IsNullOrEmpty(request.Email) && !IsValidEmail(request.Email))
        {
            throw new BadRequestException(_t["Invalid format email address"]);
        }

        var userStudent = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            AvatarUrl = request.AvatarUrl,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            StudentCode = request.StudentCode,
        };

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

        var userClass = new UserClass
        {
            ClassesId = request.ClassesId,
            StudentId = userStudent.Id
        };

        classroom.AddUserInClass(userClass);
        await _classRepository.UpdateAsync(classroom);


        // Gán các assignment của lớp học cho học sinh mới
        var classroomToAssign = await _classRepository.FirstOrDefaultAsync(new ClassesByIdSpec(request.ClassesId))
            ?? throw new NotFoundException(_t["Class not found", request.ClassesId]);
        var listAssignmentIds = classroomToAssign.AssignmentClasses.Select(x => x.AssignmentId).ToList();

        foreach (var assignmentId in listAssignmentIds)
        {
            await _mediator.Send(new AssignAssignmentToStudentsRequest
            {
                AssignmentId = assignmentId,
                StudentIds = new List<Guid> { userStudent.Id }
            });
        }
        return userStudent.Id;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Sử dụng biểu thức chính quy để kiểm tra định dạng email
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}