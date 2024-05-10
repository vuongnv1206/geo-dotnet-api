using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class NewsCommentByParentIdSpec : Specification<News>, ISingleResultSpecification
{
    public NewsCommentByParentIdSpec(Guid newsId)
    {
        Query.Where(p => p.ParentId == newsId);
    }
}
