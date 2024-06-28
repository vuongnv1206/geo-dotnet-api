using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.Comments.Dto;
public class CommentDto : IDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
    public Guid? ParentId { get; set; }
    public DateTime Timestamp { get; set; }
    public int NumberLikeInComment { get; set; }
}
