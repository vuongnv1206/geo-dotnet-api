using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Dtos;
public class QuestionFolderPermissionDto : AuditableEntity, IDto
{
    public DefaultIdType UserId { get; set; }
    public DefaultIdType QuestionFolderId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}
