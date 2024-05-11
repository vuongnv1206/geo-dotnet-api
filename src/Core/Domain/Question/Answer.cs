namespace FSH.WebApi.Domain.Question;

public class Answer : AuditableEntity, IAggregateRoot
{
    public string? Content { get; set; }
    public Guid? QuestionId { get; set; }
    public virtual Question? Question { get; set; }
    public bool IsCorrect { get; set; }
}