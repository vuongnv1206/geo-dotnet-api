using FSH.WebApi.Application.Questions.QuestionLabel.Dtos;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class SearchQuestionLabelRequest : PaginationFilter, IRequest<PaginationResponse<QuestionLabelDto>>
{
}

public class SearchQuestionLabelRequestHandler : IRequestHandler<SearchQuestionLabelRequest, PaginationResponse<QuestionLabelDto>>
{
    private readonly IReadRepository<QuestionLable> _repository;

    public SearchQuestionLabelRequestHandler(IReadRepository<QuestionLable> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<QuestionLabelDto>> Handle(SearchQuestionLabelRequest request, CancellationToken cancellationToken)
    {
        var spec = new QuestionLabelBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}
