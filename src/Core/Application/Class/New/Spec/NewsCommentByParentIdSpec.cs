using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Spec;
public class NewsCommentByParentIdSpec : Specification<News>, ISingleResultSpecification
{
    public NewsCommentByParentIdSpec(DefaultIdType newsId)
    {
        Query.Where(p => p.ParentId == newsId);
    }
}
