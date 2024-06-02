using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Dto;
public class NewsDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ClassesId { get; set; }
    public string? Content { get; set; }
    public bool IsLockComment { get; set; }
    public DefaultIdType ParentId { get; set; }
    public string? ClassesName { get; set; }
}
