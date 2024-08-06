using FSH.WebApi.Domain.Question.Enums;


namespace FSH.WebApi.Application.Examination.Matrices;
public class ContentMatrixDto
{
    public Guid QuestionFolderId { get; set; }
    public List<CriteriaQuestionDto> CriteriaQuestions { get; set; }
    public float TotalPoint { get; set; }
}
