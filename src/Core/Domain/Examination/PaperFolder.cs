using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Examination;
public class PaperFolder : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? SubjectId { get; set; }
    //public virtual Subject Subject { get; set; }
    [ForeignKey(nameof(ParentId))]
    public virtual PaperFolder? PaperFolderParent { get; set; }
    public virtual IEnumerable<PaperFolder>? PaperFolderChildrens { get; set; }

    public PaperFolder Update(string name, Guid? parentId, Guid? subjectId)
    {
        Name= name;
        ParentId= parentId;
        SubjectId = subjectId;

        return this;
    }
}
