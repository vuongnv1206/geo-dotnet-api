using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Class;
public class Comment : AuditableEntity, IAggregateRoot
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime Timestamp { get; set; }
    public virtual Post Post { get; set; }
    public virtual ICollection<CommentLikes> CommentLikes { get; set; }

    public Comment(Guid postId, Guid userId, string content, Guid? parentId, DateTime timestamp)
    {
        PostId = postId;
        UserId = userId;
        Content = content;
        ParentId = parentId;
        Timestamp = timestamp;
    }
}
