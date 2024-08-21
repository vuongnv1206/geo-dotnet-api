using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.Monitor;
public class AllStudentCanAccessPaperSpec : Specification<Paper>, ISingleResultSpecification
{
    public AllStudentCanAccessPaperSpec(DefaultIdType paperId)
    {
        _ = Query.
            Include(p => p.PaperAccesses).
            Where(p => p.Id == paperId);
    }
}