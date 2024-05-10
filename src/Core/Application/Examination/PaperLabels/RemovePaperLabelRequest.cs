using FSH.WebApi.Domain.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class RemovePaperLabelRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public RemovePaperLabelRequest(Guid id)
    {
        Id = id;
    }
}

public class RemovePaperLabelRequestHandler : IRequestHandler<RemovePaperLabelRequest, Guid>
{
    private readonly IRepositoryWithEvents<PaperLable> _repo;
    private readonly IStringLocalizer _t;

    public RemovePaperLabelRequestHandler(
        IRepositoryWithEvents<PaperLable> repo,
        IStringLocalizer<RemovePaperLabelRequestHandler> t)
    {
        _repo = repo;
        _t = t;
    }

    public async Task<DefaultIdType> Handle(RemovePaperLabelRequest request, CancellationToken cancellationToken)
    {
        var label = await _repo.GetByIdAsync(request.Id, cancellationToken);
        _ = label ?? throw new NotFoundException(_t["Label {0} Not Found.", request.Id]);

        await _repo.DeleteAsync(label, cancellationToken);

        return label.Id;
    }
}
