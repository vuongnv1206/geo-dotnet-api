using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FSH.WebApi.Domain.Class;
public class Comment : AuditableEntity, IAggregateRoot
{
    public Guid? PostId { get; set; }
    public string Content { get; set; }
    public Guid? ParentId { get; set; }
    public virtual Post Post { get; set; }
    public virtual List<CommentLikes> CommentLikes { get; set; }

    [ForeignKey(nameof(ParentId))]
    public virtual Comment? CommentParent { get; set; }
    public virtual List<Comment> CommentChildrens { get; set; } = new();

    public Comment()
    {
    }

    public Comment(Guid postId, string content, Guid? parentId)
    {
        PostId = postId;
        Content = content;
        ParentId = parentId;
    }

    public Comment Update(Guid postId, string content, Guid? parentId)
    {
        if (PostId != Guid.Empty && !PostId.Equals(postId)) PostId = postId;
        if (content is not null && Content?.Equals(content) is not true) Content = content;
        if (ParentId != Guid.Empty && !ParentId.Equals(parentId)) ParentId = parentId;
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
