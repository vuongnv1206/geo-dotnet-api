using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Examination.Monitor;

public class StudentByClassIdSpec : Specification<Classes>, ISingleResultSpecification
{
    public StudentByClassIdSpec(Guid? classId)
    {
        _ = Query.
            Include(p => p.UserClasses).
            ThenInclude(p => p.Student).
            Where(p => p.Id == classId);
    }
}