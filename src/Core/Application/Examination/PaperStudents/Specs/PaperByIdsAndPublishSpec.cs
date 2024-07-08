using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperStudents;
public class PaperByIdsAndPublishSpec : EntitiesByPaginationFilterSpec<Paper>
{
    // joinedClassIds: get all joined class
    // studentIdsInClass: get id of student store in class
    public PaperByIdsAndPublishSpec(GetPendingTestOfStudentRequest request, List<Guid> joinedClassIds, List<Guid> studentIdsInClass,Guid userId)
        : base(request)
    {
        // get paper: exist in papepIds, published, student hasn't started yet.
        Query
            .Include(p => p.PaperAccesses)
            .Where(p => p.IsPublish
                    && p.PaperAccesses.Any(pa => (pa.UserId.HasValue && studentIdsInClass.Contains(pa.UserId.Value))
                    || (pa.ClassId.HasValue && joinedClassIds.Contains(pa.ClassId.Value))))
            .Include(p => p.PaperLabel)
            .Include(p => p.Subject)
            .Include(p => p.SubmitPapers);

        if (request.CompletionStatus == CompletionStatusEnum.NotStarted)
        {
            Query.Where(p => !p.SubmitPapers.Any(x => x.CreatedBy == userId));
        }
        else if (request.CompletionStatus == CompletionStatusEnum.InProgress)
        {
            Query.Where(p => p.SubmitPapers.Any(x => x.CreatedBy == userId && !x.EndTime.HasValue));
        }
        else if (request.CompletionStatus == CompletionStatusEnum.Completed)
        {
            Query.Where(p => p.SubmitPapers.Any(x => x.CreatedBy == userId && x.EndTime.HasValue));
        }
    }
}
