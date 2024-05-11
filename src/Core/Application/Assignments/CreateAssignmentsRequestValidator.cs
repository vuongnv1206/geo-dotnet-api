using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Subjects;

namespace FSH.WebApi.Application.Assignments;

public class CreateAssignmentsRequestValidator : CustomValidator<CreateAssignmentRequest>
{
    public CreateAssignmentsRequestValidator(IReadRepository<Assignment> assignmentRepo, IReadRepository<Subject> subjectRepo, IStringLocalizer<CreateAssignmentsRequestValidator> T)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await assignmentRepo.FirstOrDefaultAsync(new AssignmentByNameSpec(name), ct) is null)
                .WithMessage((_, name) => T["Assignment {0} already Exists.", name]);

        RuleFor(p => p.Attachment);

        RuleFor(p => p.SubjectId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await subjectRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => T["Subject {0} Not Found.", id]);
    }
}
