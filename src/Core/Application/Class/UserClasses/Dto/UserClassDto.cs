using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses.Dto;
public class UserClassDto
{
    public DefaultIdType ClassesId { get; set; }
    public DefaultIdType UserStudentId { get; set; }
    public List<UserStudent> userStudents { get; set; }
    public UserClassDto(DefaultIdType userStudentId, DefaultIdType classesId)
    {
        UserStudentId = userStudentId;
        ClassesId = classesId;
    }
}
