using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class UpdatePaperLabelRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class UpdatePaperLabelRequestValidator : CustomValidator<UpdatePaperLabelRequest>
{
    public UpdatePaperLabelRequestValidator(IRepository<PaperLabel> repository, IStringLocalizer<UpdatePaperLabelRequestValidator> T) =>
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (label, name, ct) =>
                    await repository.FirstOrDefaultAsync(new PaperLabelByNameSpec(name), ct)
                        is not PaperLabel existingLabel || existingLabel.Id == label.Id)
                .WithMessage((_, name) => T["PaperLabel {0} already Exists.", name]);
}

public class UpdatePaperLabelRequestHandler : IRequestHandler<UpdatePaperLabelRequest, Guid>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<PaperLabel> _repository;
    private readonly IStringLocalizer _t;

    public UpdatePaperLabelRequestHandler(IRepositoryWithEvents<PaperLabel> repository, IStringLocalizer<UpdatePaperLabelRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);

    public async Task<Guid> Handle(UpdatePaperLabelRequest request, CancellationToken cancellationToken)
    {
        var label = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = label
        ?? throw new NotFoundException(_t["Label {0} Not Found.", request.Id]);

        label.Update(request.Name);

        await _repository.UpdateAsync(label, cancellationToken);

        return request.Id;
    }
}