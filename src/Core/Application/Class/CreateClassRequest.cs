using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class;
public class CreateClassRequest : IRequest<Guid>
{
    public required string Name { get; set; }
    public required string SchoolYear { get; set; }
    public Guid GroupClassId { get; set; }
}

public class CreateClassRequestValidator : CustomValidator<CreateClassRequest>
{
    public CreateClassRequestValidator(IReadRepository<Classes> classRepos, IReadRepository<GroupClass> groupClassRepos, IStringLocalizer<CreateClassRequestValidator> T)
    {
        RuleFor(g => g.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (name, ct) => await classRepos.FirstOrDefaultAsync(new ClassByNameSpec(name), ct) is null)
            .WithMessage((_, name) => T["Class {0} already Exists.", name]);

        RuleFor(p => p.GroupClassId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await groupClassRepos.GetByIdAsync(id, ct) is not null)
            .WithMessage((_, id) => T["GroupClass {0} Not Found.", id]);
    }
}

public class CreateClassRequestHandler : IRequestHandler<CreateClassRequest, Guid>
{
    private readonly IRepositoryWithEvents<Classes> _repository;
    private readonly ICurrentUser _currentUser;

    public CreateClassRequestHandler(IRepositoryWithEvents<Classes> repository, ICurrentUser currentUser) =>
        (_repository, _currentUser) = (repository, currentUser);

    public async Task<Guid> Handle(CreateClassRequest request, CancellationToken cancellationToken)
    {
        var user = _currentUser.GetUserId();

        var classes = new Classes(request.Name, request.SchoolYear, user, request.GroupClassId);

        await _repository.AddAsync(classes);

        return classes.Id;
    }
}