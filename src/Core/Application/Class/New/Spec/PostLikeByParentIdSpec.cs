using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Spec;
public class PostLikeByParentIdSpec : Specification<Post>, ISingleResultSpecification
{
    public PostLikeByParentIdSpec(DefaultIdType postId)
    {
        Query.Where(p => p.ParentId == postId);
    }
}
