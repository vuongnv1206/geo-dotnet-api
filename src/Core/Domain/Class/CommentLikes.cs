using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Class;
public class CommentLikes
{
    public Guid CommentId { get;  set; }
    public Guid UserId { get;  set; }
    public virtual Comment Comment { get;  set; }

    public CommentLikes()
    {
    }

    public CommentLikes(Guid commentId, Guid userId)
    {
        CommentId = commentId;
        UserId = userId;
    }
}
