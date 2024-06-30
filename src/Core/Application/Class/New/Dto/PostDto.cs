using FSH.WebApi.Application.Class.Comments.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Dto;
public class PostDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ClassesId { get; set; }
    public string? Content { get; set; }
    public bool IsLockComment { get; set; }
    public DefaultIdType ParentId { get; set; }
    public string? ClassesName { get; set; }
    public int? NumberLikeInThePost { get; set; }
    public List<CommentDto> Comments { get; set; }
    public Guid CreatedBy { get; set; }

}
