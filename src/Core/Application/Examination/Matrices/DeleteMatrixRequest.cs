using FSH.WebApi.Domain.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Matrices;
public class DeleteMatrixRequest : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteMatrixRequest(Guid id)
    {
        Id = id;
    }
}
public class DeleteMatrixRequestHandler : IRequestHandler<DeleteMatrixRequest, Guid>
{
    private readonly IRepository<PaperMatrix> _repositoryMatrix;
    private readonly IStringLocalizer<DeleteMatrixRequestHandler> _t;

    public DeleteMatrixRequestHandler(
        IRepository<PaperMatrix> repositoryMatrix,
        IStringLocalizer<DeleteMatrixRequestHandler> t)
    {
        _repositoryMatrix = repositoryMatrix;
        _t = t;
    }

    public async Task<Guid> Handle(DeleteMatrixRequest request, CancellationToken cancellationToken)
    {
        var matrix = await _repositoryMatrix.GetByIdAsync(request.Id, cancellationToken);
        if (matrix == null)
        {
            throw new NotFoundException(_t["Matrix {0} Not Found.", request.Id]);
        }
        await _repositoryMatrix.DeleteAsync(matrix, cancellationToken);

        return matrix.Id;
    }
}
