using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public interface IQuestionService : ITransientService
{
    Task<QuestionFolder> GetRootFolder(Guid folderId, CancellationToken cancellationToken);
    Task<List<Guid>> GetMyFolderIds(Guid userId, CancellationToken cancellationToken);
    Task<List<Guid>> GetFolderIds(Guid folderId, CancellationToken cancellationToken);
    Task<int> countQuestions(Guid folderId, CancellationToken cancellationToken);
    Task<List<Guid>> ChangeQuestionStatus(Guid userId, List<Guid> questionId, QuestionStatus status, CancellationToken cancellationToken);
    Task<List<Guid>> RestoreDeletedQuestions(Guid userId, List<Guid> questionIds, CancellationToken cancellationToken);
    Task<List<Guid>> DeleteQuestions(Guid userId, List<Guid> questionIds, CancellationToken cancellationToken);
}
