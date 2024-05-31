using FSH.WebApi.Application.Identity.Users;

namespace FSH.WebApi.Application.Questions.Dtos;
public class QuestionTreeDto : AuditableEntity, IDto
{
    public new DefaultIdType Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DefaultIdType? ParentId { get; private set; }
    public UserDetailsDto? Owner { get; set; }
    public bool CurrentShow { get; set; }
    public int TotalQuestions { get; set; }
    public ICollection<QuestionFolderPermissionDto>? Permission { get; set; }
    public ICollection<QuestionTreeDto>? Children { get; set; }
}
