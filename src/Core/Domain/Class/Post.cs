using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Class;
public class Post : AuditableEntity, IAggregateRoot
{
    public string Content { get; private set; }
    public bool IsLockComment { get; private set; }
    public Guid ClassesId { get; private set; }
    public virtual Classes Classes { get; private set; }
    public virtual ICollection<Comment> Comments { get; private set; }
    public virtual ICollection<PostLike> PostLikes { get; private set; }

    public Post()
    {
    }

    public Post(string content, bool isLockComment, Guid classesId)
    {
        Content = content;
        IsLockComment = isLockComment;
        ClassesId = classesId;
    }

    public Post Update(string? content, bool? isLockComment)
    {
        if (content is not null && !Content.Equals(content)) Content = content;
        if (isLockComment.HasValue) IsLockComment = isLockComment.Value;
        return this;
    }
}
