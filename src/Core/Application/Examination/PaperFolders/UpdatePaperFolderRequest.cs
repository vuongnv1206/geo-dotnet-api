namespace FSH.WebApi.Application.Examination.PaperFolders;
public class UpdatePaperFolderRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; }
}
