using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments.Spec;
internal class CommentTreeSpec : Specification<Comment>
{
    public CommentTreeSpec()
    {
        Query.Include(x => x.CommentLikes).Include(x => x.CommentChildrens);
    }
}
