using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.UserStudents;
public class UpdateInformationStudentRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? StudentCode { get; set; }
    public bool? Gender { get; set; }
}


public class UpdateInformationStudentRequestHandler : IRequestHandler<UpdateInformationStudentRequest, Guid>
{
    private readonly IRepositoryWithEvents<Student> _userStudentRepo;
    private readonly IStringLocalizer _t;

    public UpdateInformationStudentRequestHandler(IRepositoryWithEvents<Student> userStudentRepo,
                                                  IStringLocalizer<UpdateInformationStudentRequestHandler> t)
    {
        _userStudentRepo = userStudentRepo;
        _t = t;
    }

    public async Task<Guid> Handle(UpdateInformationStudentRequest request, CancellationToken cancellationToken)
    {
        var userStudent = await _userStudentRepo.GetByIdAsync(request.Id);
        if (userStudent is null)
            throw new NotFoundException(_t["Student {0} Not Found.", request.Id]);

        if (request.Email is not null && ! userStudent.IsValidEmail(request.Email))
            throw new ConflictException(_t["The email address '{0}' is not valid.", request.Email]);

        if (request.PhoneNumber is not null && !userStudent.IsValidPhoneNumber(request.PhoneNumber))
            throw new ConflictException(_t["The phone number '{0}' is not valid. It must be 10 digits.", request.PhoneNumber]);

        var existDuplicate = await _userStudentRepo.AnyAsync(new StudentByStudentCodeSpec(request.StudentCode));
        if (existDuplicate)
        {
            throw new ConflictException(_t["The student code '{0}' is already in use.", request.StudentCode]);
        }
        userStudent.Update(request.FirstName, request.LastName, request.Email, request.PhoneNumber,request.DateOfBirth,request.StudentCode, request.Gender);

        await _userStudentRepo.UpdateAsync(userStudent, cancellationToken);
        return request.Id;
    }
}
