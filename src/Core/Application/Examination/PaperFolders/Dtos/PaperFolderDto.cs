using FSH.WebApi.Domain.Examination;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatorName { get; set; }
    public DateTime CreatedOn { get; private set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public List<PaperFolderDto>? PaperFolderChildrens { get; set; }
    public List<PaperFolderParentDto>? Parents { get; set; }
    public List<PaperFolderPermissionDto> PaperFolderPermissions { get; set; }
}
