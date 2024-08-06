
namespace FSH.WebApi.Application.Examination.Matrices;
public class PaperMatrixDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // Format : [{QuestionFolderId},{Criteria:[{QuestionLabelId},{QuestionType},{NumberOfQuestion},{RawIndex}]},{TotalPoint}]
    public string Content { get; set; }
    public List<ContentMatrixDto> ContentItems { get; set; }
    public float TotalPoint { get; set; }
}
