using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses.Dto;
public class GroupClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Name { get; set; }
}
