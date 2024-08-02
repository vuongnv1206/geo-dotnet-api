namespace FSH.WebApi.Application.Examination.PaperLabels;
public class PaperLabelDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}
