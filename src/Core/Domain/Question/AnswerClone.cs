

namespace FSH.WebApi.Domain.Question;
public class AnswerClone : AuditableEntity, IAggregateRoot
{
    public string? Content { get; set; }
    public Guid? QuestionCloneId { get; set; }
    public virtual QuestionClone? QuestionClone { get; set; }
    public bool IsCorrect { get; set; }

    public AnswerClone()
    {
        
    }
    public AnswerClone(string? content, Guid? questionCloneId, bool isCorrect)
    {
        Content = content;
        QuestionCloneId = questionCloneId;
        IsCorrect = isCorrect;
    }

    public AnswerClone(string? content, bool isCorrect)
    {
        Content = content;
        IsCorrect = isCorrect;
    }

    public void Update(string? content, bool isCorrect, Guid questionCloneId)
    {
        Content = content;
        IsCorrect = isCorrect;
        QuestionCloneId = questionCloneId;
    }   
}

