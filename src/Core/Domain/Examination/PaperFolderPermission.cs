using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Examination;
public class PaperFolderPermission : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; set; }
    public Guid FolderId { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    [ForeignKey(nameof(FolderId))]
    public virtual PaperFolder PaperFolder { get; set; }
}
