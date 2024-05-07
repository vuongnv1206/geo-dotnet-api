using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class QuestionTreeDto : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; private set; }

    public bool CurrentShow { get; set; }
    public List<QuestionFolderPermissionDto> Permission { get; set; } = new();
    public List<QuestionTreeDto>? Children { get; set; } = new();
}
