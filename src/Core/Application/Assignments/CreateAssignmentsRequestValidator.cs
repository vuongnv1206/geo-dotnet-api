using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Application.Assignments;

public class CreateAssignmentsRequestValidator : CustomValidator<CreateAssignmentRequest>
{
    public CreateAssignmentsRequestValidator(IReadRepository<Assignment> productRepo, IReadRepository<Brand> brandRepo, IStringLocalizer<CreateAssignmentsRequestValidator> T)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => await productRepo.FirstOrDefaultAsync(new AssignmentByNameSpec(name), ct) is null)
                .WithMessage((_, name) => T["Assignment {0} already Exists.", name]);

        //RuleFor(p => p.Rate)
        //    .GreaterThanOrEqualTo(1);

        RuleFor(p => p.Image);

        RuleFor(p => p.SubjectId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await brandRepo.GetByIdAsync(id, ct) is not null)
                .WithMessage((_, id) => T["Brand {0} Not Found.", id]);
    }
}