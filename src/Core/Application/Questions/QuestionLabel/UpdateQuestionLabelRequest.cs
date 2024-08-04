using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class UpdateQuestionLabelRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}

public class UpdateQuestionLabelRequestValidator : CustomValidator<UpdateQuestionLabelRequest>
{
    public UpdateQuestionLabelRequestValidator(IReadRepository<QuestionLable> repository, IStringLocalizer<UpdateQuestionLabelRequestValidator> T) =>
        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(50)
        .MustAsync(async (name, ct) => await repository.AnyAsync(new QuestionLabelByNameSpec(name), ct) is false)
            .WithMessage((_, name) => T["Question label name {0} already Exists.", name]);
}

public class UpdateQuestionLabelRequestHandler : IRequestHandler<UpdateQuestionLabelRequest, Guid>
{
    private readonly IRepositoryWithEvents<QuestionLable> _repository;

    public UpdateQuestionLabelRequestHandler(
        IRepositoryWithEvents<QuestionLable> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(UpdateQuestionLabelRequest request, CancellationToken cancellationToken)
    {
        var questionLabel = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = questionLabel ?? throw new NotFoundException($"QuestionLabel {request.Id} Not Found.");

        questionLabel.Update(request.Name, request.Color);

        await _repository.UpdateAsync(questionLabel, cancellationToken);

        return questionLabel.Id;
    }
}
