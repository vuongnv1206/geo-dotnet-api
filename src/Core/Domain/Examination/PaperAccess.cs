

using FSH.WebApi.Domain.Class;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Examination;
public class PaperAccess : BaseEntity<Guid>
{
    public Guid PaperId { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? UserId { get; set; }
    public virtual Paper Paper { get; set; }

    [ForeignKey(nameof(ClassId))]
    public virtual Classes Class { get; set; }
}
