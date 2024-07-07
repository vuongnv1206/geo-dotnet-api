using DocumentFormat.OpenXml.InkML;
using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FSH.WebApi.Infrastructure.Question;
public class QuestionService : IQuestionService
{
    private readonly ApplicationDbContext _repository;
    private readonly IDapperRepository _dapperRepository;


    public async Task<QuestionFolder> GetRootFolder(Guid folderId, CancellationToken cancellationToken)
    {
        var folder = await _repository.QuestionFolders.FindAsync(folderId, cancellationToken);

        if (folder == null) throw new NotFoundException("Folder not found.");

        while (folder.ParentId != null)
        {
            folder = await _repository.QuestionFolders.FindAsync(folder.ParentId, cancellationToken);
        }

        return folder;
    }

    public QuestionService(ApplicationDbContext repository, IDapperRepository dapperRepository)
    {
        _repository = repository;
        _dapperRepository = dapperRepository;
    }

    public async Task<List<Guid>> GetMyFolderIds(Guid userId, CancellationToken cancellationToken)
    {
        List<Guid> rootFolderIds = await _repository.QuestionFolders
            .Where(f => f.CreatedBy == userId && f.ParentId == null)
            .Select(f => f.Id)
            .ToListAsync();
        List<Guid> myFolderIds = new List<Guid>();
        foreach (Guid rootFolderId in rootFolderIds)
        {
            List<Guid> folderIds = await GetFolderIds(rootFolderId, cancellationToken);
            myFolderIds.AddRange(folderIds);
        }

        return myFolderIds;
    }

    public async Task<List<Guid>> GetFolderIds(Guid folderId, CancellationToken cancellationToken)
    {
        const string sql = @"
            WITH RECURSIVE RecursiveFolders AS (
                SELECT ""Id""
                FROM ""Question"".""QuestionFolders""
                WHERE ""Id"" = @p0

                UNION ALL

                SELECT qf.""Id""
                FROM ""Question"".""QuestionFolders"" qf
                INNER JOIN RecursiveFolders rf ON qf.""ParentId"" = rf.""Id""
            )
            SELECT rf.""Id""
            FROM RecursiveFolders rf;
        ";

        IReadOnlyList<Guid> folderIds = await _dapperRepository.RawQueryAsync<Guid>(sql, new { p0 = folderId }, cancellationToken: cancellationToken);
        return folderIds.ToList();
    }

    public async Task<List<Guid>> ChangeQuestionStatus(Guid userId, List<Guid> questionId, QuestionStatus status, CancellationToken cancellationToken)
    {
        using (IDbContextTransaction transaction = _repository.Database.BeginTransaction())
        {
            try
            {
                foreach (Guid id in questionId)
                {
                    var question = await _repository.Questions
                        .Where(q => q.Id.Equals(id) && q.QuestionFolderId.HasValue)
                        .FirstOrDefaultAsync(cancellationToken);
                    if (question == null) throw new NotFoundException($"Question {id} not found.");
                    if (question.QuestionStatus != QuestionStatus.Pending) throw new BadRequestException($"Question {id} is not pending.");
                    var rootFolder = await GetRootFolder(question.QuestionFolderId!.Value, cancellationToken);
                    if (!rootFolder.CreatedBy.Equals(userId)) throw new ForbiddenException($"You are not allowed to change the status of Question {id}");
                    question.QuestionStatus = status;
                    _repository.Update(question);
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

        return questionId;
    }
}
