using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;
public class UpdateAssignmentRequestValidator : CustomValidator<UpdateAssignmentRequest>
{
    //public UpdateAssignmentRequestValidator(IReadRepository<Assignment> assignmentRepo, IReadRepository<Subject> subjectRepo, IStringLocalizer<UpdateAssignmentRequestValidator> T)
    //{
    //    RuleFor(p => p.Name)
    //        .NotEmpty()
    //        .MaximumLength(75)
    //        .MustAsync(async (assignment, name, ct) =>
    //                await assignmentRepo.FirstOrDefaultAsync(new AssignmentByNameSpec(name), ct)
    //                    is not Assignment existingAssignment || existingAssignment.Id == assignment.Id)
    //            .WithMessage((_, name) => T["Assignment {0} already Exists.", name]);

    //    //RuleFor(p => p.Rate)
    //    //    .GreaterThanOrEqualTo(1);

    //    RuleFor(p => p.Image);

    //    RuleFor(p => p.SubjectId)
    //        .NotEmpty()
    //        .MustAsync(async (id, ct) => await subjectRepo.GetByIdAsync(id, ct) is not null)
    //            .WithMessage((_, id) => T["Subject {0} Not Found.", id]);
    //}
}