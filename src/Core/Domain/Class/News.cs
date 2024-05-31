using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Class;
public class News : AuditableEntity, IAggregateRoot
{
    public string Content { get; private set; }
    public bool IsLockComment { get; private set; }
    public Guid? ParentId { get; private set; }
    public Guid ClassesId { get; private set; }
    public virtual Classes Classes { get; private set; }
    [ForeignKey(nameof(ParentId))]
    public virtual News? NewsParent { get; private set; }

    public News(string content, bool isLockComment, Guid? parentId, Guid classesId)
    {
        Content = content;
        IsLockComment = isLockComment;
        ParentId = parentId;
        ClassesId = classesId;
    }

    public News Update(string? content, bool? isLockCommnet, Guid? parentId)
    {
        if (isLockCommnet.HasValue) IsLockComment = isLockCommnet.Value;
        if (content is not null && Content?.Equals(content) is not true) Content = content;
        if (parentId.HasValue && parentId.Value != Guid.Empty && !ParentId.Equals(parentId.Value)) ParentId = parentId.Value;
        return this;
    }
}
