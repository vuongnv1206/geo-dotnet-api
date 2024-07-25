using FSH.WebApi.Application.Questions;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class QuestionStatisticDto : IDto
{
    public Guid Id { get; set; }
    public Guid? PaperId { get; set; }
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public string? QuestionFolderName { get; set; }
    public virtual QuestionType? QuestionType { get; set; }
    public Guid? QuestionLabelId { get; set; }
    public string? QuestionLabelName { get; set; }
    public Guid? QuestionParentId { get; set; }
    public List<QuestionStatisticDto>? QuestionPassages { get; set; }
    public List<AnswerDto>? Answers { get; set; }
    public int? RawIndex { get; set; }
    public int TotalTest { get; set; }
    public int TotalAnswered { get; set; }
    public int TotalNotAnswered
    {
        get
        {
            return TotalTest - TotalAnswered;
        }
    }

    public int TotalCorrect { get; set; }
    public int TotalWrong { get; set; }
    public List<StudentInfo>? WrongStudents { get; set; } = new();
}

