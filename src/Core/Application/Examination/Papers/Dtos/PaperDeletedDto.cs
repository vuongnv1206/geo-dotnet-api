using FSH.WebApi.Application.Subjects;

namespace FSH.WebApi.Application.Examination.Papers.Dtos;
public class PaperDeletedDto : IDto
{
    public Guid Id { get; set; }
    public string ExamName { get; set; }
    public SubjectDto Subject { get; set; }
    public int Duration { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
}
