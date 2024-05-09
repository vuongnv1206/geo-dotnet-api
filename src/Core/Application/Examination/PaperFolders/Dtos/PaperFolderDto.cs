namespace FSH.WebApi.Application.Examination.PaperFolders.Dtos;
public class PaperFolderDto : IDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? SubjectId { get; set; }
    public List<PaperFolderDto>? PaperFolderChildrens { get; set; }
}
