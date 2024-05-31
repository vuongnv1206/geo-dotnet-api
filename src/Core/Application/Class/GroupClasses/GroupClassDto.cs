using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.GroupClasses;
public class GroupClassDto : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public List<ClassDto>? Classes { get; set; } = new();
}
