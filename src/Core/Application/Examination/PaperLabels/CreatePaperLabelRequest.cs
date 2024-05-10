using FSH.WebApi.Domain.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class CreatePaperLabelRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
}

public class CreatePaperLabelRequestValidator : CustomValidator<CreatePaperLabelRequest>
{
    public CreatePaperLabelRequestValidator(IReadRepository<PaperLable> repository, IStringLocalizer<CreatePaperLabelRequestValidator> T) =>
        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(50)
        .MustAsync(async (name, ct) => await repository.AnyAsync(new SearchPaperLabelByNameSpec(name), ct) is false)
            .WithMessage((_, name) => T["Paper label name {0} already Exists.", name]);
}

public class CreatePaperLabelRequestHandler : IRequestHandler<CreatePaperLabelRequest, Guid>
{
    private readonly IRepositoryWithEvents<PaperLable> _repository;

    public CreatePaperLabelRequestHandler(
        IRepositoryWithEvents<PaperLable> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreatePaperLabelRequest request, CancellationToken cancellationToken)
    {
        var paperLabel = new PaperLable(request.Name);

        await _repository.AddAsync(paperLabel, cancellationToken);

        return paperLabel.Id;
    }
}
