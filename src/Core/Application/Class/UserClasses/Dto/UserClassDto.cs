using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses.Dto;
public class UserClassDto
{
    public DefaultIdType ClassesId { get; set; }
    public DefaultIdType UserId { get; set; }
    public UserClassDto(DefaultIdType userId, DefaultIdType classesId)
    {
        UserId = userId;
        ClassesId = classesId;
    }
}
