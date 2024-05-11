using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Dtos;
public class QuestionTreeDto : AuditableEntity, IDto
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DefaultIdType? ParentId { get; private set; }

    public bool CurrentShow { get; set; }
    public List<QuestionFolderPermissionDto> Permission { get; set; } = new();
    public List<QuestionTreeDto>? Children { get; set; } = new();
}
