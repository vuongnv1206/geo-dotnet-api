using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Domain.Question;

public class Question : AuditableEntity, IAggregateRoot
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public virtual QuestionFolder? QuestionFolder { get; set; }
    public virtual QuestionType? QuestionType { get; set; }
    public Guid? QuestionLableId { get; set; }
    public virtual QuestionLable? QuestionLable { get; set; }
    public Guid? ParentId { get; set; }
    public virtual Question? Parent { get; set; }
    public virtual List<Question> Passages { get; set; } = new();
    public virtual List<Answer> Answers { get; set; } = new();
}