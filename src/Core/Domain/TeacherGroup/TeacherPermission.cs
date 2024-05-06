using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.TeacherGroup;
public class TeacherPermission : AuditableEntity,IAggregateRoot
{
    public Guid ClassId { get; set; }
    public Guid TeacherId { get; set; }
    public bool CanAssignExamination { get; set; }
    public bool CanMarking { get; set; }
    public bool CanManagementStudent { get; set; }
    //public virtual Class Class { get; set; }


}
