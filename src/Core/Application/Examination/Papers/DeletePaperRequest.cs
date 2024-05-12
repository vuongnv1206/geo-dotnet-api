using FSH.WebApi.Domain.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Papers;
public class DeletePaperRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public DeletePaperRequest(Guid id)
    {
        Id = id;
    }
}

public class DeletePaperRequestHandler : IRequestHandler<DeletePaperRequest, Guid>
{
    private readonly IRepositoryWithEvents<Paper> _repo;
    private readonly IStringLocalizer _t;

    public DeletePaperRequestHandler(
        IRepositoryWithEvents<Paper> repo,
        IStringLocalizer<DeletePaperRequestHandler> t)
    {
        _repo = repo;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(DeletePaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repo.GetByIdAsync(request.Id);
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        await _repo.DeleteAsync(paper);

        return paper.Id;
    }
}
