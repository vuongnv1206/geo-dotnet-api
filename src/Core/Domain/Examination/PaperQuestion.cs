using FSH.WebApi.Domain.Question;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Examination;
public class PaperQuestion
{
    public Guid PaperId { get; set; }
    public Guid QuestionId { get; set; }
    public float Mark { get; set; }
    public int? RawIndex { get; set; }
    public virtual Paper? Paper { get; set; }
    [ForeignKey(nameof(QuestionId))]
    public virtual QuestionClone? Question { get; set; }
}
