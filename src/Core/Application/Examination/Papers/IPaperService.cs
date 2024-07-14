using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public interface IPaperService : ITransientService
{
    Task<PaperFolder> GetRootFolder(Guid folderId, CancellationToken cancellationToken);
    Task<List<Guid>> RestoreDeletedPapers(Guid userId, List<Guid> paperIds, CancellationToken cancellationToken);
    Task<List<Guid>> DeletePapers(Guid userId, List<Guid> paperIds, CancellationToken cancellationToken);
}
