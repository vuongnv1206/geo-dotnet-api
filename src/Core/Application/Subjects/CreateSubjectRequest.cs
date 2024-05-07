using FSH.WebApi.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Subjects;
public class CreateSubjectRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class CreateSubjectRequestValidator : CustomValidator<CreateSubjectRequest>
{
    public CreateSubjectRequestValidator(IReadRepository<Subject> repository, IStringLocalizer<CreateSubjectRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await repository.FirstOrDefaultAsync(new SubjectByNameSpec(name), ct) is null)
                .WithMessage((_, name) => T["Subject {0} already Exists.", name]);
}

public class CreateSubjectRequestHandler : IRequestHandler<CreateSubjectRequest, Guid>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Subject> _repository;

    public CreateSubjectRequestHandler(IRepositoryWithEvents<Subject> repository) => _repository = repository;

    public async Task<Guid> Handle(CreateSubjectRequest request, CancellationToken cancellationToken)
    {
        var subject = new Subject(request.Name, request.Description);

        await _repository.AddAsync(subject, cancellationToken);

        return subject.Id;
    }
}