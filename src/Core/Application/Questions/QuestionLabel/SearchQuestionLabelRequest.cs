using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class SearchQuestionLabelRequest : PaginationFilter, IRequest<PaginationResponse<QuestionLabelDto>>
{
}

public class SearchQuestionLabelRequestHandler : IRequestHandler<SearchQuestionLabelRequest, PaginationResponse<QuestionLabelDto>>
{
    private readonly IReadRepository<QuestionLable> _repository;

    public SearchQuestionLabelRequestHandler(IReadRepository<QuestionLable> repository) => _repository = repository;

    public async Task<PaginationResponse<QuestionLabelDto>> Handle(SearchQuestionLabelRequest request, CancellationToken cancellationToken)
    {
        var spec = new QuestionLabelBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);
    }
}
