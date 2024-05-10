using FSH.WebApi.Domain.Subjects;

namespace FSH.WebApi.Application.Subjects;
public class SubjectByNameSpec : Specification<Subject>, ISingleResultSpecification
{
    public SubjectByNameSpec(string name) =>
        Query.Where(b => b.Name == name);
}