

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperAccessDto
{
    public Guid Id { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? UserId { get; set; }
}
