using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class NewsDto : IDto
{
    public Guid Id { get;  set; }
    public Guid ClassesId { get;  set; }
    public string? Content { get;  set; }
    public bool IsLockComment { get;  set; }
    public Guid ParentId { get;  set; }
    public string? ClassesName { get;  set; }
}
