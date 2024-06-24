namespace FSH.WebApi.Domain.Question;

public class Answer : AuditableEntity, IAggregateRoot
{
    public string? Content { get; set; }
    public Guid? QuestionId { get; set; }
    public virtual Question? Question { get; set; }
    public bool IsCorrect { get; set; }

    public Answer()
    {
        
    }
    public Answer(string? content, Guid? questionId, bool isCorrect)
    {
        Content = content;
        QuestionId = questionId;
        IsCorrect = isCorrect;
    }

    public Answer(string? content, bool isCorrect)
    {
        Content = content;
        IsCorrect = isCorrect;
    }

    public void Update(string? content, bool isCorrect, Guid questionId)
    {
        Content = content;
        IsCorrect = isCorrect;
        QuestionId = questionId;
    }
}