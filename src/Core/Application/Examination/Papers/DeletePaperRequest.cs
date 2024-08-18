using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.PaperFolders;
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
    private readonly IRepository<PaperFolder> _repositoryFolder;
    private readonly ICurrentUser _currentUser;
    public DeletePaperRequestHandler(
        IRepositoryWithEvents<Paper> repo,
        IStringLocalizer<DeletePaperRequestHandler> t,
        ICurrentUser currentUser,
        IRepository<PaperFolder> repositoryFolder)
    {
        _repo = repo;
        _t = t;
        _currentUser = currentUser;
        _repositoryFolder = repositoryFolder;
    }

    public async Task<DefaultIdType> Handle(DeletePaperRequest request, CancellationToken cancellationToken)
    {
        var paper = await _repo.FirstOrDefaultAsync(new PaperByIdSpec(request.Id));
        _ = paper ?? throw new NotFoundException(_t["Paper {0} Not Found.", request.Id]);

        if (!paper.CanDelete(_currentUser.GetUserId()))
        {
            throw new ForbiddenException(_t["You do not have permission to delete this paper."]);
        }

        await _repo.DeleteAsync(paper);

        return paper.Id;
    }
}
