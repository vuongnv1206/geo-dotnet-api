using System.ComponentModel;

namespace FSH.WebApi.Domain.Question;

public class QuestionFolder : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public Guid? ParentId { get; private set; }
    public virtual QuestionFolder? Parent { get; private set; }

    public virtual List<QuestionFolder> Children { get; private set; } = new();

    public virtual List<QuestionFolderPermission> Permissions { get; private set; } = new();

    public QuestionFolder(string name, Guid? parentId)
    {
        Name = name;
        ParentId = parentId;
    }

    public void AddChild(QuestionFolder questionFolder)
    {
        Children.Add(questionFolder);
    }

}