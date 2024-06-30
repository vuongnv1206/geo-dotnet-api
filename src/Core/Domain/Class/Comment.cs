using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FSH.WebApi.Domain.Class;
public class Comment : AuditableEntity, IAggregateRoot
{
    public Guid? PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime Timestamp { get; set; }
    public virtual Post Post { get; set; }
    public virtual ICollection<CommentLikes> CommentLikes { get; set; }

    public Comment()
    {
    }

    public Comment(Guid postId, Guid userId, string content, Guid? parentId, DateTime timestamp)
    {
        PostId = postId;
        UserId = userId;
        Content = content;
        ParentId = parentId;
        Timestamp = timestamp;
    }

    public Comment Update(Guid postId, Guid userId, string content, Guid? parentId, DateTime timestamp)
    {
        if (PostId != Guid.Empty && !PostId.Equals(postId)) PostId = postId;
        if (UserId != Guid.Empty && !UserId.Equals(userId)) UserId = userId;
        if (content is not null && Content?.Equals(content) is not true) Content = content;
        if (ParentId != Guid.Empty && !ParentId.Equals(parentId)) ParentId = parentId;
        if (Timestamp != DateTime.MinValue && !Timestamp.Equals(timestamp)) Timestamp = timestamp;
        return this;
    }

    public void AddCommentLike(CommentLikes commentLikes)
    {
        CommentLikes.Add(commentLikes);
    }

    public void RemoveCommentLike(CommentLikes commentLikes)
    {
        CommentLikes.Remove(commentLikes);
    }
}
