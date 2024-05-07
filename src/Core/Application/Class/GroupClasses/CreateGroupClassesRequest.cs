using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class CreateGroupClassesRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
}

public class CreateGroupClassRequestValidator : CustomValidator<CreateGroupClassesRequest>
{
    public CreateGroupClassRequestValidator(IReadRepository<GroupClass> repository, IStringLocalizer<CreateGroupClassRequestValidator> T) {
        RuleFor(g => g.Name)
            .NotEmpty()
            .MaximumLength(256)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new GroupClassByNameSpec(name), ct) is null)
            .WithMessage((_, name) => T["GroupClass {0} already Exists.", name]);
    }
}

public class CreateGroupClassRequestHandler : IRequestHandler<CreateGroupClassesRequest, Guid>
{
    private readonly IRepositoryWithEvents<GroupClass> _repository;

    public CreateGroupClassRequestHandler(IRepositoryWithEvents<GroupClass> repository) => _repository = repository;

    public async Task<Guid> Handle(CreateGroupClassesRequest request, CancellationToken cancellationToken)
    {
        var groupClasses = new GroupClass(request.Name);

        await _repository.AddAsync(groupClasses);

        return groupClasses.Id;
    }
}