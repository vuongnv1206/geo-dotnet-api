

namespace FSH.WebApi.Application.Examination.Papers;
public class CreateUpdateQuestionInPaperDto : IDto
{
    public Guid QuestionId { get; set; }
    public float Mark { get; set; }
}
