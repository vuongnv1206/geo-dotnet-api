using FSH.WebApi.Application.Class.UserStudents.Spec;
using FSH.WebApi.Domain.Class;
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
    private readonly IRepository<UserStudent> _userStudentRepository;
    private readonly IStringLocalizer<DeleteStudentRequestHandler> _localizer;
    public DeleteStudentRequestHandler(IStringLocalizer<DeleteStudentRequestHandler> localizer, IRepository<UserStudent> repository)
    {
        _localizer = localizer;
        _userStudentRepository = repository;
    }

    public async Task<Guid> Handle(DeleteStudentRequest request, CancellationToken cancellationToken)
    {
        var student = await _userStudentRepository.FirstOrDefaultAsync(new UserStudentByIdSpec(request.Id));

        _ = student ?? throw new NotFoundException(_localizer["Student in class {0} Not Found."]);

        await _userStudentRepository.DeleteAsync(student, cancellationToken);
        return student.Id;
    }
}   
