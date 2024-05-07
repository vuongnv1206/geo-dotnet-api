using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FSH.WebApi.Domain.Class;
public class News : AuditableEntity, IAggregateRoot
{
    public Guid ClassId { get; private set; }
    public string Content { get; private set; }
    public bool IsLockCommnet { get; private set; }
    public Guid ParentId { get; private set; }
    public virtual Classes Classes { get; private set; }

    public News(Guid classId, string? content, bool isLockCommnet, Guid parentId)
    {
        ClassId = classId;
        Content = content;
        IsLockCommnet = isLockCommnet;
        ParentId = parentId;
    }

    public News Update(Guid? classId, string? content, bool? isLockCommnet, Guid? parentId)
    {
        if (isLockCommnet.HasValue) IsLockCommnet = isLockCommnet.Value;
        if (content is not null && Content?.Equals(content) is not true) Content = content;
        if (classId.HasValue && classId.Value != Guid.Empty && !ClassId.Equals(classId.Value)) ClassId = classId.Value;
        if (parentId.HasValue && parentId.Value != Guid.Empty && !ParentId.Equals(parentId.Value)) ParentId = parentId.Value;
        return this;
    }
}
