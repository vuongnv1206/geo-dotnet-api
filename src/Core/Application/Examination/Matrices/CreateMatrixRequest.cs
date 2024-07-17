
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
    private readonly IRepository<QuestionFolder> _paperFolderRepo;
    private readonly IMediator _mediator;

    public CreateMatrixRequestHandler(IRepository<PaperMatrix> repositoryMatrix, IStringLocalizer<CreateMatrixRequestHandler> t, IRepository<QuestionFolder> paperFolderRepo, IMediator mediator)
    {
        _repositoryMatrix = repositoryMatrix;
        _t = t;
        _paperFolderRepo = paperFolderRepo;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreateMatrixRequest request, CancellationToken cancellationToken)
    {
        var matrix = new PaperMatrix(request.Name, request.Content, request.TotalPoint);
        await _repositoryMatrix.AddAsync(matrix);
      
        return matrix.Id;
    }
}
