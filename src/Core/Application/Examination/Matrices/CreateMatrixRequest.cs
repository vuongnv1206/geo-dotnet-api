
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
        // Deserialize content từ JSON string thành List<ContentMatrixDto>
        var contentItems = _serializerService.Deserialize<List<ContentMatrixDto>>(request.Content);

        // Khởi tạo tập hợp để lưu các RawIndex đã tồn tại
        var allRawIndexes = new HashSet<int>();

        // Duyệt qua từng ContentMatrixDto trong danh sách
        foreach (var item in contentItems)
        {
            // Kiểm tra QuestionFolderId có hợp lệ hay không
            var questionFolderTree = await _questionFolderRepo.ListAsync(
                new QuestionFolderTreeSpec(), cancellationToken);
            var questionFolder = questionFolderTree.Where(x => x.Id == item.QuestionFolderId).FirstOrDefault();

            
            _ = questionFolder ?? throw new NotFoundException(
                $"Question Folder with ID {item.QuestionFolderId} not found.");

            // Remove CriteriaQuestion where NumberOfQuestion is 0
            item.CriteriaQuestions = item.CriteriaQuestions.Where(c => c.NumberOfQuestion > 0).ToList();



            // Duyệt qua từng tiêu chí (criteria) trong CriteriaQuestions của ContentMatrixDto
            foreach (var criteria in item.CriteriaQuestions)
            {
                // Kiểm tra xem QuestionLabelId có tồn tại hay không
                var questionLabelExists = await _repositoryQuestionLabel.FirstOrDefaultAsync(
                    new QuestionLabelByIdSpec(criteria.QuestionLabelId), cancellationToken);
                _ = questionLabelExists ?? throw new NotFoundException(
                    $"Question Label with ID {criteria.QuestionLabelId} not found.");

                // Lấy tổng số câu hỏi trong folder with label
                var totalQuestionsAlreadyExisted = questionFolder.CountQuestionWithLabelInFolder(criteria.QuestionLabelId);


                // Kiểm tra nếu NumberOfQuestion lớn hơn tổng số câu hỏi có sẵn trong folder
                if (criteria.NumberOfQuestion > totalQuestionsAlreadyExisted)
                {
                    throw new ConflictException($"Cannot get {criteria.NumberOfQuestion} questions for  {questionLabelExists.Name} level in '{questionFolder.Name}' folder as only {totalQuestionsAlreadyExisted} are available");
                }

                // Nếu RawIndex trống, tính toán và gán giá trị mới cho RawIndex
                if (string.IsNullOrWhiteSpace(criteria.RawIndex))
                {
                    var maxIndex = allRawIndexes.Any() ? allRawIndexes.Max() : 0;
                    var missingIndexes = Enumerable.Range(1, maxIndex).Except(allRawIndexes).ToList();
                    var newIndexes = missingIndexes.Take(criteria.NumberOfQuestion).ToList();
                    if (newIndexes.Count < criteria.NumberOfQuestion)
                    {
                        newIndexes.AddRange(
                            Enumerable.Range(maxIndex + 1, criteria.NumberOfQuestion - newIndexes.Count));
                    }
                    criteria.RawIndex = string.Join(",", newIndexes);
                }

                // Chuyển đổi RawIndex từ string thành danh sách các số nguyên
                var indexes = criteria.RawIndex.Split(',').Select(int.Parse).ToList();

                // Kiểm tra và thêm từng RawIndex vào tập hợp allRawIndexes, nếu bị trùng sẽ ném ngoại lệ
                foreach (var index in indexes)
                {
                    if (!allRawIndexes.Add(index))
                    {
                        throw new ConflictException($"RawIndex '{index}' is duplicated across CriteriaQuestions.");
                    }
                }
            }
        }

        var matrix = new PaperMatrix(request.Name, request.Content, request.TotalPoint);
        await _repositoryMatrix.AddAsync(matrix);

        return matrix.Id;
    }

}
