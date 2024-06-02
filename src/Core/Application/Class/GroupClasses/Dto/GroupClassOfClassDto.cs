using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses.Dto;
public class GroupClassOfClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Name { get; set; }
    public List<ClassDto> Classes { get; set; }
}
