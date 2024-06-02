using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;

namespace FSH.WebApi.Application.Questions.Dtos;
public class QuestionFolderPermissionDto : AuditableEntity, IDto
{
    public DefaultIdType UserId { get; set; }
    public UserDetailsDto? User { get; set; }
    public DefaultIdType GroupTeacherId { get; set; }
    public GroupTeacherDto? GroupTeacher { get; set; }
    public DefaultIdType QuestionFolderId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanShare { get; set; }
}
