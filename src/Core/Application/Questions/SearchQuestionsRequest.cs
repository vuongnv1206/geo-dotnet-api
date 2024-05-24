using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using Mapster;

namespace FSH.WebApi.Application.Questions;

public class SearchQuestionsRequest : PaginationFilter, IRequest<PaginationResponse<QuestionDto>>
{
    public Guid? FolderId { get; set; }
    public string? Content { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLableId { get; set; }
}

public class SearchQuestionsRequestHandler : IRequestHandler<SearchQuestionsRequest, PaginationResponse<QuestionDto>>
{
    private readonly IReadRepository<Question> _repository;

    public SearchQuestionsRequestHandler(IReadRepository<Question> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<QuestionDto>> Handle(SearchQuestionsRequest request, CancellationToken cancellationToken)
    {
        var spec = new QuestionsBySearchRequestSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}