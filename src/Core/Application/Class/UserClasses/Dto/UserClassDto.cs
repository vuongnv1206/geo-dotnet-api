using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.UserClasses.Dto;
public class UserClassDto
{
    public Guid ClassesId { get; set; }
    public Guid StudentId { get; set; }
    public List<Student> UserStudents { get; set; }
}
