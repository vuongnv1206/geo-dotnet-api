using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments.Spec;
public class CommentByIdSpec : Specification<Comment>
{
    public CommentByIdSpec(Guid id)
    {
        Query.Where(x => x.Id == id).Include(x => x.CommentLikes);
    }
}
