
namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderTreeDto
{
    public Guid Id { get; set; }
    public int TotalPapers { get; set; }
    public List<PaperFolderPermissionDto>? PaperFolderPermissions { get; set; }
    public List<PaperFolderDto> PaperFolderChildrens { get; set; }
}
