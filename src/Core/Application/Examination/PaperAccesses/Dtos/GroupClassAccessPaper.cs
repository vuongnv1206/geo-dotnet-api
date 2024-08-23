using FSH.WebApi.Application.Class.Dto;

namespace FSH.WebApi.Application.Examination.PaperAccesses;
public class GroupClassAccessPaper
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public ClassAccessPaper[]? Classes { get; set; }
}

public class ClassAccessPaper
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? GroupClassName { get; set; }
    public StudentAccessPaper[]? UserClasses { get; set; }
}

public class StudentAccessPaper
{
    public Guid StudentId { get; set; }
    public StudentDto Student { get; set; }
    public string? GroupName { get; set; }
    public string? ClassName { get; set; }
}
