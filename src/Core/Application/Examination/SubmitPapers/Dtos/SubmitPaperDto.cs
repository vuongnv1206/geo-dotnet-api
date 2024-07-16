namespace FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
public class SubmitPaperDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PaperId { get; set; }
    public required string Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public float TotalMark { get; set; }
    public DefaultIdType CreatedBy { get; set; }
    public string? CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
}

public class SubmitPaperAnswer
{
    public string? Content { get; set; }
    public string? Id { get; set; }
    public bool IsCorrect { get; set; }
    public string? QuestionId { get; set; }
}

public class SubmitPaperQuestion
{
    public List<SubmitPaperAnswer>? Answers { get; set; }
    public string? Id { get; set; }
    public List<SubmitPaperQuestion>? QuestionPassages { get; set; }
}

public class SubmitPaperData
{
    public string? DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceType { get; set; }
    public string? Id { get; set; }
    public string? LocalIp { get; set; }
    public List<SubmitPaperQuestion>? Questions { get; set; }
    public string? SubmitPaperId { get; set; }
}
