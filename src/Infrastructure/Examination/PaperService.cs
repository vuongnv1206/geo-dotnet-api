using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FSH.WebApi.Infrastructure.Examination;
public class PaperService : IPaperService
{
    private readonly ApplicationDbContext _repository;

    public PaperService(ApplicationDbContext repository)
    {
        _repository = repository;
    }

    public async Task<PaperFolder> GetRootFolder(Guid folderId, CancellationToken cancellationToken)
    {
        var folder = await _repository.PaperFolders.FindAsync(folderId, cancellationToken);
        if (folder == null) throw new NotFoundException("Folder not found.");

        while (folder.ParentId != null)
        {
            folder = await _repository.PaperFolders.FindAsync(folder.ParentId, cancellationToken);
        }

        return folder;

    }

    public async Task<List<DefaultIdType>> RestoreDeletedPapers(Guid userId, List<Guid> paperIds, CancellationToken cancellationToken)
    {
        using (IDbContextTransaction transaction = _repository.Database.BeginTransaction())
        {
            try
            {
                foreach (Guid paperId in paperIds)
                {
                    var paper = await _repository.Papers.IgnoreQueryFilters()
                        .Where(p => p.Id.Equals(paperId))
                        .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException("Paper not found.");

                    if (!await CanDeletePaper(paper, userId, cancellationToken)) throw new BadRequestException("You are not allowed to delete this paper.");

                    paper.DeletedBy = null;
                    paper.DeletedOn = null;

                    _repository.Update(paper);
                    _repository.SaveChanges();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        return paperIds;
    }

    public async Task<List<DefaultIdType>> DeletePapers(Guid userId, List<Guid> paperIds, CancellationToken cancellationToken)
    {
        using (IDbContextTransaction transaction = _repository.Database.BeginTransaction())
        {
            try
            {
                foreach (Guid paperId in paperIds)
                {
                    var paper = await _repository.Papers.IgnoreQueryFilters()
                        .Include(p => p.PaperQuestions).Include(p => p.PaperAccesses).Include(p => p.PaperPermissions)
                        .Where(p => p.Id.Equals(paperId))
                        .FirstAsync(cancellationToken) ?? throw new NotFoundException("Paper not found.");

                    if (!await CanDeletePaper(paper, userId, cancellationToken)) throw new BadRequestException("You are not allowed to delete this paper.");

                    _repository.Remove(paper);
                    _repository.SaveChanges();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        return paperIds;
    }

    private async Task<bool> CanDeletePaper(Paper paper, Guid userId, CancellationToken cancellationToken)
    {
        if (_repository.SubmitPapers.Any(sp => sp.PaperId.Equals(paper.Id))) throw new BadRequestException("Paper is already submitted by student.");
        if (paper.CreatedBy.Equals(userId)) return true;
        if (paper.PaperFolderId == null) return false;
        var rootFolder = await GetRootFolder(paper.PaperFolderId!.Value, cancellationToken);
        return rootFolder.CreatedBy.Equals(userId);
    }

}
