using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserStudents;
public class UpdateStudentWhenUserRegisterRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public Guid? StudentId { get; set; }
}

public class UpdateStudentWhenUserRegisterRequestHandler : IRequestHandler<UpdateStudentWhenUserRegisterRequest, Guid>
{
    private readonly IRepositoryWithEvents<Student> _repository;
    private readonly IStringLocalizer _t;

    public UpdateStudentWhenUserRegisterRequestHandler(IRepositoryWithEvents<Student> repository, IStringLocalizer<UpdateStudentWhenUserRegisterRequestHandler> localizer)
        => (_repository, _t) = (repository, localizer);

    public async Task<Guid> Handle(UpdateStudentWhenUserRegisterRequest request, CancellationToken cancellationToken)
    {
        var userStudent = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (userStudent == null)
            throw new NotFoundException(_t["UserStudent{0} Not Found.", request.Id]);

        userStudent.StId = request.StudentId;

        await _repository.UpdateAsync(userStudent, cancellationToken);

        return request.Id;
    }
}
