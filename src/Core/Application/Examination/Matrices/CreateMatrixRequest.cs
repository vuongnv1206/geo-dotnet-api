
using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.In.Examination.Matrices.Helpers;


namespace FSH.WebApi.Application.Examination.Matrices;
public class CreateMatrixRequest : IRequest<Guid>
{
    public string Name { get; set; }
    // Format : [{QuestionFolderId},{Criteria:[{QuestionLabelId},{QuestionType},{NumberOfQuestion},{RawIndex}]},{TotalPoint}]
    [ContentFormat]
    public string Content { get; set; }
    public float TotalPoint { get; set; }
}

//[
//    {
//        "QuestionFolderId": "GUID",
//        "Criteria": [
//            {
//                "QuestionLabelId": "GUID",
//                "QuestionType": "Enum",
//                "NumberOfQuestion": 3,
//                "RawIndex": "1,2,3"
//            },
//            ...
//        ],
//        "TotalPoint": 10.0
//    },
//    ...
//]


public class CreateMatrixRequestHandler : IRequestHandler<CreateMatrixRequest, Guid>
{
    private readonly IRepository<PaperMatrix> _repositoryMatrix;
    private readonly IStringLocalizer<CreateMatrixRequestHandler> _t;
    private readonly IRepository<QuestionFolder> _questionFolderRepo;
    private readonly IMediator _mediator;
    private readonly ISerializerService _serializerService;
    private readonly IRepository<QuestionLable> _repositoryQuestionLabel;
    public CreateMatrixRequestHandler(IRepository<PaperMatrix> repositoryMatrix, IStringLocalizer<CreateMatrixRequestHandler> t, IRepository<QuestionFolder> questionFolderRepo, IMediator mediator, ISerializerService serializerService, IRepository<QuestionLable> repositoryQuestionLabel)
    {
        _repositoryMatrix = repositoryMatrix;
        _t = t;
        _questionFolderRepo = questionFolderRepo;
        _mediator = mediator;
        _serializerService = serializerService;
        _repositoryQuestionLabel = repositoryQuestionLabel;
    }

    public async Task<Guid> Handle(CreateMatrixRequest request, CancellationToken cancellationToken)
    {
        var contentItems = _serializerService.Deserialize<List<ContentMatrixDto>>(request.Content);

        foreach (var contentMatrixDto in contentItems)
        {
            // Kiểm tra QuestionFolderId có hợp lệ hay không
            var questionFolder = await _questionFolderRepo.FirstOrDefaultAsync(new QuestionFolderByIdSpec(contentMatrixDto.QuestionFolderId), cancellationToken);
            _ = questionFolder ?? throw new NotFoundException($"Question Folder with ID {contentMatrixDto.QuestionFolderId} not found.");

            var totalQuestionsInFolder = questionFolder.CountQuestionInFolder(); // Tổng số câu hỏi trong folder

            foreach (var criteria in contentMatrixDto.CriteriaQuestions)
            {
                var questionLabelExists = await _repositoryQuestionLabel.FirstOrDefaultAsync(new QuestionLabelByIdSpec(criteria.QuestionLabelId), cancellationToken);
                _ = questionLabelExists ?? throw new NotFoundException($"Question Label with ID {criteria.QuestionLabelId} not found.");

                // Kiểm tra nếu NumberOfQuestion > tổng số câu hỏi có sẵn
                if (criteria.NumberOfQuestion > totalQuestionsInFolder)
                {
                    throw new ValidationException($"NumberOfQuestion ({criteria.NumberOfQuestion}) for Label ID {criteria.QuestionLabelId} exceeds available questions ({totalQuestionsInFolder}) in the folder.");
                }
            }
        }

        var matrix = new PaperMatrix(request.Name, request.Content, request.TotalPoint);
        await _repositoryMatrix.AddAsync(matrix);

        return matrix.Id;
    }

}
