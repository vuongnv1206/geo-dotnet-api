
using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Matrices;
public class MyPaperMatrixSpec : Specification<PaperMatrix>
{
    public MyPaperMatrixSpec(Guid currentUserId)
    {
        Query.Where(x => x.CreatedBy == currentUserId);
    }
}
