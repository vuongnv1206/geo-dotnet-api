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
    public Guid UserStudentId { get; set; }
    public virtual UserStudent UserStudent { get;  set; }
    public virtual Classes Classes { get; set; }

    public UserClass()
    {
    }

    public UserClass(Guid classesId, Guid userStudentId)
    {
        ClassesId = classesId;
        UserStudentId = userStudentId;
    }

}
