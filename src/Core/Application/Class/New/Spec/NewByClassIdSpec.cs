using FSH.WebApi.Domain.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Spec;
public class NewByClassIdSpec : Specification<News>, ISingleResultSpecification
{
    public NewByClassIdSpec(Guid classId) => Query.Where(b => b.ClassesId == classId);
}
