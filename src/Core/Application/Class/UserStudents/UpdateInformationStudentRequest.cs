using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Domain.Class;

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

    public UpdateInformationStudentRequestHandler(
        IRepositoryWithEvents<Student> repository,
        IStringLocalizer<UpdateInformationStudentRequestHandler> t)
    {
        _repository = repository;
        _t = t;
    }

    public async Task<Guid> Handle(UpdateInformationStudentRequest request, CancellationToken cancellationToken)
    {
        var student = await _repository.FirstOrDefaultAsync(new StudentByIdSpec(request.Id), cancellationToken);
        _ = student ?? throw new NotFoundException(_t["Student {0} Not Found.", request.Id]);

        //if (request.Email is not null && !student.IsValidEmail(request.Email))
        //    throw new ConflictException(_t["The email address '{0}' is not valid.", request.Email]);
        //if (request.PhoneNumber is not null && !student.IsValidPhoneNumber(request.PhoneNumber))
        //    throw new ConflictException(_t["The phone number '{0}' is not valid. It must be 10 digits.", request.PhoneNumber]);

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
