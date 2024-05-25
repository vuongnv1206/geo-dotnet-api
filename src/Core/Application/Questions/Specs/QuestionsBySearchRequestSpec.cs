using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;

public class QuestionsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Domain.Question.Question, QuestionDto>
{
    public QuestionsBySearchRequestSpec(SearchQuestionsRequest request)
    : base(request) =>
        Query
        .Include(q => q.QuestionPassages)
        .OrderBy(c => c.CreatedOn, !request.HasOrderBy())
        .Where(q => q.QuestionFolderId.Equals(request.FolderId!.Value), request.FolderId.HasValue)
        .Where(q => q.Content.Contains(request.Content!), !string.IsNullOrEmpty(request.Content))
        .Where(q => q.QuestionType == request.QuestionType, request.QuestionType.HasValue)
        .Where(q => q.QuestionLableId.Equals(request.QuestionLableId!.Value), request.QuestionLableId.HasValue);
}