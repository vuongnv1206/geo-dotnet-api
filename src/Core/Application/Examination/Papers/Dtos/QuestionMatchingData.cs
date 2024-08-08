namespace FSH.WebApi.Application.Examination.Papers;
public class QuestionMatchingData
{
    public string? Question { get; set; }
    public Dictionary<string, string>? ColumnA { get; set; }
    public Dictionary<string, string>? ColumnB { get; set; }
}
