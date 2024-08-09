using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Papers;
public class PapersByClassIdSpec : Specification<Paper>
{
    public PapersByClassIdSpec(Guid classId)
    {
        Query.Include(pa => pa.PaperAccesses)
            .Where(pa => pa.PaperAccesses.Any(pa => pa.ClassId == classId));

    }
}
