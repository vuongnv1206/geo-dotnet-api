namespace FSH.WebApi.Application.Subjects;
public class SubjectDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

}