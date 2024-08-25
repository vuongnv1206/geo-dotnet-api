using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using Mapster;

namespace FSH.WebApi.Application.Questions;
public class GetQuestionRandomRequest : IRequest<List<QuestionDto>>
{
    public Guid FolderId { get; set; }
    public QuestionType QuestionType { get; set; }
    public Guid QuestionLabelId { get; set; }
    public int NumberQuestion { get; set; }

    public GetQuestionRandomRequest(
        DefaultIdType folderId,
        QuestionType questionType,
        DefaultIdType questionLabelId,
        int numberQuestion)
    {
        FolderId = folderId;
        QuestionType = questionType;
        QuestionLabelId = questionLabelId;
        NumberQuestion = numberQuestion;
    }
}

public class GetQuestionRandomRequestHandler : IRequestHandler<GetQuestionRandomRequest, List<QuestionDto>>
{
    private readonly IReadRepository<Domain.Question.Question> _questionRepo;
    private readonly IRepository<QuestionFolder> _questionFolderRepo;

    public GetQuestionRandomRequestHandler(
        IReadRepository<Domain.Question.Question> questionRepo,
        IRepository<QuestionFolder> questionFolderRepo)
    {
        _questionRepo = questionRepo;
        _questionFolderRepo = questionFolderRepo;
    }

    public async Task<List<QuestionDto>> Handle(GetQuestionRandomRequest request, CancellationToken cancellationToken)
    {
        var rootFolder = await _questionFolderRepo.FirstOrDefaultAsync(new QuestionFolderByIdSpec(request.FolderId));
        var folderIds = new List<Guid> { request.FolderId };
        rootFolder.ChildQuestionFolderIds(rootFolder.Children, folderIds);

        var spec = new QuestionFromMatrixSpec(folderIds, request.QuestionLabelId, request.QuestionType);
        var questions = await _questionRepo.ListAsync(spec, cancellationToken);

        if (questions.Count < request.NumberQuestion)
        {
            throw new BadRequestException("Not enough questions found.");
        }

        var random = new Random();
        var questionRandom = questions.OrderBy(x => random.Next()).Take(request.NumberQuestion).ToList();

        return questionRandom.Adapt<List<QuestionDto>>();
    }
}
