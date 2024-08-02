
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class CreateQuestionLabelRequest : IRequest<Guid>
{
    public string Name { get; set; }
    public string Color { get; set; }
}

public class CreateQuestionLabelRequestValidator : AbstractValidator<CreateQuestionLabelRequest>
{
    public CreateQuestionLabelRequestValidator(IReadRepository<QuestionLable> repository, IStringLocalizer<CreateQuestionLabelRequestValidator> T)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
             .MustAsync(async (name, ct) => await repository.AnyAsync(new QuestionLabelByNameSpec(name), ct) is false)
            .WithMessage((_, name) => T["Question label name {0} already Exists.", name]); 
        RuleFor(x => x.Color).NotEmpty().MaximumLength(10);
    }
}

public class CreateQuestionLabelRequestHandler : IRequestHandler<CreateQuestionLabelRequest, Guid>
{
    private readonly IRepository<QuestionLable> _repository;

    public CreateQuestionLabelRequestHandler(IRepository<QuestionLable> repository)
    {
       _repository = repository;
    }

    public async Task<Guid> Handle(CreateQuestionLabelRequest request, CancellationToken cancellationToken)
    {
        var questionLabel = new QuestionLable
        {
            Name = request.Name,
            Color = request.Color
        };

        await _repository.AddAsync(questionLabel, cancellationToken);

        return questionLabel.Id;
    }
}

