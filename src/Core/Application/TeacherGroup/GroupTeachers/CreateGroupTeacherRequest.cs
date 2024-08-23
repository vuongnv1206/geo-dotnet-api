using FSH.WebApi.Application.TeacherGroup.QRCode;
using FSH.WebApi.Domain.TeacherGroup;
using System.Data.Common;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class CreateGroupTeacherRequest : IRequest<Guid>
{
    public string Name { get; set; }
}

public class CreateGroupTeacherRequestValidator : CustomValidator<CreateGroupTeacherRequest>
{
    public CreateGroupTeacherRequestValidator(IReadRepository<GroupTeacher> repository, IStringLocalizer<CreateGroupTeacherRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new GroupTeacherByNameSpec(name), ct) is null)
                .WithMessage((_, name) => T["GroupTeacher {0} already Exists.", name]);

}

public class CreateGroupTeacherRequestHandler : IRequestHandler<CreateGroupTeacherRequest, Guid>
{
    private readonly IRepositoryWithEvents<GroupTeacher> _repository;

    public CreateGroupTeacherRequestHandler(
        IRepositoryWithEvents<GroupTeacher> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreateGroupTeacherRequest request, CancellationToken cancellationToken)
    {
        var groupTeacher = new GroupTeacher(request.Name);
        await _repository.AddAsync(groupTeacher, cancellationToken);

        Dictionary<string, string> data = new Dictionary<string, string>
        {
            { "id", groupTeacher.Id.ToString() }
        };

        groupTeacher.UpdateJoinGroup($"join-group/{groupTeacher.Id}");

        await _repository.UpdateAsync(groupTeacher, cancellationToken);

        return groupTeacher.Id;
    }
}
