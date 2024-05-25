namespace FSH.WebApi.Domain.Question;

public class Answer : AuditableEntity, IAggregateRoot
{
    public string? Content { get; set; }
    public Guid? QuestionId { get; set; }
    public virtual Question? Question { get; set; }
    public bool IsCorrect { get; set; }

    public Answer(string? content, Guid? questionId, bool isCorrect)
    {
        Content = content;
        QuestionId = questionId;
        IsCorrect = isCorrect;
    }

    public Answer Update(string? content, bool isCorrect)
    {
        Content = content;
        IsCorrect = isCorrect;

        return this;
    }
}