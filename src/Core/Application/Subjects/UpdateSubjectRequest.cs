using FSH.WebApi.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Subjects;
public class UpdateSubjectRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class UpdateSubjectRequestValidator : CustomValidator<UpdateSubjectRequest>
{
    public UpdateSubjectRequestValidator(IRepository<Subject> repository, IStringLocalizer<UpdateSubjectRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (subject, name, ct) =>
                    await repository.FirstOrDefaultAsync(new SubjectByNameSpec(name), ct)
                        is not Subject existingSubject || existingSubject.Id == subject.Id)
                .WithMessage((_, name) => T["Subject {0} already Exists.", name]);
}

public class UpdateSubjectRequestHandler : IRequestHandler<UpdateSubjectRequest, Guid>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Subject> _repository;
    private readonly IStringLocalizer _t;

    public UpdateSubjectRequestHandler(IRepositoryWithEvents<Subject> repository, IStringLocalizer<UpdateSubjectRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<Guid> Handle(UpdateSubjectRequest request, CancellationToken cancellationToken)
    {
        var subject = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = subject
        ?? throw new NotFoundException(_t["Subject {0} Not Found.", request.Id]);

        subject.Update(request.Name, request.Description);

        await _repository.UpdateAsync(subject, cancellationToken);

        return request.Id;
    }
}