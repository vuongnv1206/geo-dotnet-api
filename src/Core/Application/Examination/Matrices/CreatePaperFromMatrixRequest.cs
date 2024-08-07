using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Examination.Matrices;
public class CreatePaperFromMatrixRequest : IRequest<List<QuestionGenarateToMatrix>>
{
    public Guid MatrixId { get; set; }
}

public class CreatePaperFromMatrixRequestHandler : IRequestHandler<CreatePaperFromMatrixRequest, List<QuestionGenarateToMatrix>>
{
    private readonly IRepository<PaperMatrix> _matrixRepo;
    private readonly IStringLocalizer<CreatePaperFromMatrixRequestHandler> _t;
    private readonly IMediator _mediator;
    private readonly IReadRepository<QuestionFolder> _questionFolderRepo;
    private readonly ISerializerService _serializerService;

    public CreatePaperFromMatrixRequestHandler(
        IRepository<PaperMatrix> matrixRepo,
        IStringLocalizer<CreatePaperFromMatrixRequestHandler> t,
        IMediator mediator,
        IReadRepository<QuestionFolder> questionFolderRepo,
        ISerializerService serializerService)
    {
        _matrixRepo = matrixRepo;
        _t = t;
        _mediator = mediator;
        _questionFolderRepo = questionFolderRepo;
        _serializerService = serializerService;
    }

    public async Task<List<QuestionGenarateToMatrix>> Handle(CreatePaperFromMatrixRequest request, CancellationToken cancellationToken)
    {
        var matrix = await _matrixRepo.GetByIdAsync(request.MatrixId)
            ?? throw new NotFoundException(_t["Matrix {0} Not Found.", request.MatrixId]);

        var matrixContent = _serializerService.Deserialize<List<ContentMatrixDto>>(matrix.Content);

        var response = new List<QuestionGenarateToMatrix>();
        var rawIndexesPaper = new List<int>();

        foreach (var item in matrixContent)
        {
            var rootFolder = await _questionFolderRepo.FirstOrDefaultAsync(new QuestionFolderByIdSpec(item.QuestionFolderId));
            var folderIds = new List<Guid> { item.QuestionFolderId };
            rootFolder.ChildQuestionFolderIds(rootFolder.Children, folderIds);

            int totalQuestionsRequestInFolder = item.CriteriaQuestions.Sum(criteria => criteria.NumberOfQuestion);
            float markPerQuestion = item.TotalPoint / totalQuestionsRequestInFolder;

            foreach (var criteria in item.CriteriaQuestions.Where(x => x.NumberOfQuestion > 0))
            {
                var questionRequest = new GetQuestionRandomRequest(
                    folderIds,
                    criteria.QuestionType,
                    criteria.QuestionLabelId,
                    criteria.NumberOfQuestion);

                var selectedQuestions = await _mediator.Send(questionRequest);

                if (string.IsNullOrEmpty(criteria.RawIndex))
                {
                    response.AddRange(selectedQuestions.Select(question => new QuestionGenarateToMatrix
                    {
                        Question = question,
                        Mark = markPerQuestion,
                    }));
                }
                else
                {
                    var rawIndexes = criteria.RawIndex.Split(',')
                    .Select(int.Parse)
                    .ToList();

                    // Check for duplicates
                    if (rawIndexes.Any(x => rawIndexesPaper.Contains(x)))
                    {
                        throw new BadRequestException(_t["RawIndex contains duplicate values."]);
                    }

                    if (rawIndexes.Count < selectedQuestions.Count)
                    {
                        throw new BadRequestException(_t["Index question are missing compared to the total number of question"]);
                    }

                    rawIndexesPaper.AddRange(rawIndexes);

                    response.AddRange(selectedQuestions.Select((question, i) => new QuestionGenarateToMatrix
                    {
                        Question = question,
                        Mark = markPerQuestion,
                        RawIndex = rawIndexes[i]
                    }));
                }
            }
        }

        // complete raw index missing
        var allPossibleIndexes = new HashSet<int>(Enumerable.Range(1, response.Count));
        allPossibleIndexes.ExceptWith(rawIndexesPaper);

        var missingIndexes = new Queue<int>(allPossibleIndexes);

        foreach (var item in response)
        {
            if (item.RawIndex == null && missingIndexes.Count > 0)
            {
                item.RawIndex = missingIndexes.Dequeue();
            }
        }

        return response.OrderBy(x => x.RawIndex).ToList();
    }
}
