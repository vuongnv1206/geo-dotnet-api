using FSH.WebApi.Application.Questions.QuestionLabel.Dtos;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class GetQuestionLabelRequest : IRequest<QuestionLabelDto>
{
    public Guid Id { get; set; }
    public GetQuestionLabelRequest(Guid id)
    {
        Id = id;
    }
}

public class GetQuestionLabelRequestHandler : IRequestHandler<GetQuestionLabelRequest, QuestionLabelDto>
{
    private readonly IRepository<QuestionLable> _repository;
    private readonly IStringLocalizer _t;
    public GetQuestionLabelRequestHandler(IRepository<QuestionLable> repository, IStringLocalizer<GetQuestionLabelRequestHandler> localizer)
    {
        _repository = repository;
        _t = localizer;
    }

    public async Task<QuestionLabelDto> Handle(GetQuestionLabelRequest request, CancellationToken cancellationToken)
    {
        var questionLabel = await _repository.GetByIdAsync(request.Id);
        _ = questionLabel ?? throw new NotFoundException(_t["QuestionLabel {0} Not Found.", request.Id]);

        return questionLabel.Adapt<QuestionLabelDto>();
    }
}

