using FSH.WebApi.Application.Class.GroupClasses.Spec;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class UpdateGroupClassRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}

public class UpdateGroupClassRequestValidator : CustomValidator<UpdateGroupClassRequest>
{
    public UpdateGroupClassRequestValidator(
        IRepository<GroupClass> repository,
        IStringLocalizer<UpdateGroupClassRequestValidator> T)
    {
        RuleFor(g => g.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (groupClass, name, ct) => await repository.FirstOrDefaultAsync(new GroupClassByNameSpec(name), ct)
                       is not GroupClass existingGroupClass || existingGroupClass.Id == groupClass.Id)
                        .WithMessage((_, name) => T["GroupClass {0} already Exists.", name]);
    }
}

public class UpdateGroupClassRequestHandler : IRequestHandler<UpdateGroupClassRequest, Guid>
{
    private readonly IRepositoryWithEvents<GroupClass> _repository;
    private readonly IStringLocalizer _t;

    public UpdateGroupClassRequestHandler(
        IRepositoryWithEvents<GroupClass> repository,
        IStringLocalizer<UpdateGroupClassRequestHandler> localizer)
    {
        (_repository, _t) = (repository, localizer);
    }

    public async Task<DefaultIdType> Handle(UpdateGroupClassRequest request, CancellationToken cancellationToken)
    {
        var groupClass = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = groupClass ?? throw new NotFoundException(_t["GroupClass {0} Not Found.", request.Id]);

        groupClass.Update(request.Name);

        await _repository.UpdateAsync(groupClass, cancellationToken);

        return groupClass.Id;
    }
}
