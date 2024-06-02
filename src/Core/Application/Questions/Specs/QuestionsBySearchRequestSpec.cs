using FSH.WebApi.Application.Questions.Dtos;

namespace FSH.WebApi.Application.Questions.Specs;

public class QuestionsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Domain.Question.Question, QuestionDto>
{
    public QuestionsBySearchRequestSpec(SearchQuestionsRequest request, List<Guid> folderIds)
    : base(request) =>
        Query
        .Include(q => q.QuestionPassages)
        .OrderBy(c => c.CreatedOn, !request.HasOrderBy())
        .Where(q => q.QuestionFolderId.HasValue && folderIds.Contains(q.QuestionFolderId!.Value), request.folderId.HasValue)
        .Where(q => q.Content.Contains(request.Content!), !string.IsNullOrEmpty(request.Content))
        .Where(q => q.QuestionType == request.QuestionType, request.QuestionType.HasValue)
        .Where(q => q.QuestionLableId.Equals(request.QuestionLableId!.Value), request.QuestionLableId.HasValue);
}