using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.In.Examination.Matrices.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Matrices;
public class UpdateMatrixRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // Format : [{QuestionFolderId},{Criteria:[{QuestionLabelId},{QuestionType},{NumberOfQuestion},{RawIndex}]},{TotalPoint}]
    [ContentFormat]
    public string Content { get; set; }
    public float TotalPoint { get; set; }
}

public class UpdateMatrixRequestHandler : IRequestHandler<UpdateMatrixRequest, Guid>
{
    private readonly IRepository<PaperMatrix> _repositoryMatrix;
    private readonly IStringLocalizer<UpdateMatrixRequestHandler> _t;
    private readonly IReadRepository<QuestionFolder> _questionFolderRepo;
    private readonly IMediator _mediator;

    public UpdateMatrixRequestHandler(
        IRepository<PaperMatrix> repositoryMatrix,
        IStringLocalizer<UpdateMatrixRequestHandler> t,
        IReadRepository<QuestionFolder> questionFolderRepo,
        IMediator mediator)
    {
        _repositoryMatrix = repositoryMatrix;
        _t = t;
        _questionFolderRepo = questionFolderRepo;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(UpdateMatrixRequest request, CancellationToken cancellationToken)
    {
        var matrix = await _repositoryMatrix.GetByIdAsync(request.Id, cancellationToken);
        if (matrix == null)
        {
            throw new NotFoundException(_t["Matrix {0} Not Found.", request.Id]);
        }

        // Cập nhật thông tin ma trận
        matrix.Update(request.Name, request.Content, request.TotalPoint);

        await _repositoryMatrix.UpdateAsync(matrix, cancellationToken);

        return matrix.Id;
    }
}
