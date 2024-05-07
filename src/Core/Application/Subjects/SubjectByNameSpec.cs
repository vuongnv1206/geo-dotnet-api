using FSH.WebApi.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Subjects;
public class SubjectByNameSpec : Specification<Subject>, ISingleResultSpecification
{
    public SubjectByNameSpec(string name) =>
        Query.Where(b => b.Name == name);
}