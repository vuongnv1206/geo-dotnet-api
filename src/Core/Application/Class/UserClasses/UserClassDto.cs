using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses;
public class UserClassDto
{

    public Guid ClassesId { get;  set; }
    public Guid UserId { get;  set; }
    public UserClassDto(Guid userId, Guid classesId)
    {
        UserId = userId;
        ClassesId = classesId;
    }
}
