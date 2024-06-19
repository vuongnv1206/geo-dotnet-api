using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Questions;

public class SearchQuestionsRequest : PaginationFilter, IRequest<PaginationResponse<QuestionDto>>
{
    public Guid? folderId { get; set; }
    public string? Content { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLableId { get; set; }
}

public class SearchQuestionsRequestHandler : IRequestHandler<SearchQuestionsRequest, PaginationResponse<QuestionDto>>
{
    private readonly IReadRepository<Question> _repository;
    private readonly IDapperRepository _dapperRepository;
    private readonly IUserService _userService;

    public SearchQuestionsRequestHandler(IReadRepository<Question> repository, IDapperRepository dapperRepository, IUserService userService)
    {
        _repository = repository;
        _dapperRepository = dapperRepository;
        _userService = userService;
    }

    public async Task<List<Guid>> GetFolderIds(Guid folderId, CancellationToken cancellationToken)
    {
        const string sql = @"
            WITH RECURSIVE RecursiveFolders AS (
                SELECT ""Id""
                FROM ""Question"".""QuestionFolders""
                WHERE ""Id"" = @p0

                UNION ALL

                SELECT qf.""Id""
                FROM ""Question"".""QuestionFolders"" qf
                INNER JOIN RecursiveFolders rf ON qf.""ParentId"" = rf.""Id""
            )
            SELECT rf.""Id""
            FROM RecursiveFolders rf;
        ";

        IReadOnlyList<Guid> folderIds = await _dapperRepository.RawQueryAsync<Guid>(sql, new { p0 = folderId }, cancellationToken: cancellationToken);
        return folderIds.ToList();
    }

    public async Task<PaginationResponse<QuestionDto>> Handle(SearchQuestionsRequest request, CancellationToken cancellationToken)
    {
        var folderIds = await GetFolderIds(request.folderId!.Value, cancellationToken);

        var spec = new QuestionsBySearchRequestSpec(request, folderIds);
        var res = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);

        // Get owner details for each Question
        foreach (var question in res.Data)
        {
            try
            {
                var user = await _userService.GetAsync(question.CreatedBy.ToString(), cancellationToken);
                if (user != null)
                {
                    question.Owner = user;
                }
            }
            catch
            {

            }
        }

        return res;
    }
}