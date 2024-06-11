namespace FSH.WebApi.Domain.Examination;
public class PaperQuestion
{
    public Guid PaperId { get; set; }
    public Guid QuestionId { get; set; }
    public float Mark { get; set; }
    public int? RawIndex { get; set; }
    public virtual Paper? Paper { get; set; }
    public virtual Question.Question? Question { get; set; }
}
