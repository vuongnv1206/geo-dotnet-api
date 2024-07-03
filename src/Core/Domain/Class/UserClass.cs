using FSH.WebApi.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FSH.WebApi.Domain.Class;
public class UserClass
{
    public Guid ClassesId { get; set; }
    public Guid StudentId { get; set; }
    public virtual Student Student { get; set; }

    public UserClass()
    {
    }

    public UserClass(DefaultIdType classesId, DefaultIdType studentId)
    {
        ClassesId = classesId;
        StudentId = studentId;
    }
}
