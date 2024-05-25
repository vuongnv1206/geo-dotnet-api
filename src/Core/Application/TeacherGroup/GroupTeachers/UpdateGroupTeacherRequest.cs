using FSH.WebApi.Application.Catalog.Brands;
using FSH.WebApi.Domain.TeacherGroup;

namespace FSH.WebApi.Application.TeacherGroup.GroupTeachers;
public class UpdateGroupTeacherRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}

public class UpdateGroupTeacherRequestValidator : CustomValidator<UpdateGroupTeacherRequest>
{
    public UpdateGroupTeacherRequestValidator(IRepository<GroupTeacher> repository, IStringLocalizer<UpdateBrandRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (group, name, ct) =>
                    await repository.FirstOrDefaultAsync(new GroupTeacherByNameSpec(name), ct)
                        is not GroupTeacher existingBrand || existingBrand.Id == group.Id)
                .WithMessage((_, name) => T["GroupTeacher {0} already Exists.", name]);

}

public class UpdateGroupTeacherRequestHandler : IRequestHandler<UpdateGroupTeacherRequest, Guid>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<GroupTeacher> _repository;
    private readonly IStringLocalizer _t;

    public UpdateGroupTeacherRequestHandler(IRepositoryWithEvents<GroupTeacher> repository, IStringLocalizer<UpdateGroupTeacherRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<Guid> Handle(UpdateGroupTeacherRequest request, CancellationToken cancellationToken)
    {
        var group = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = group
        ?? throw new NotFoundException(_t["GroupTeacher {0} Not Found.", request.Id]);

        group.Update(request.Name);

        await _repository.UpdateAsync(group, cancellationToken);

        return request.Id;
    }
}