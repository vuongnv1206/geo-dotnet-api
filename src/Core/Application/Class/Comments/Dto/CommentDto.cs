using FSH.WebApi.Application.Identity.Users;
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
    public DefaultIdType CreatedBy { get; set; }
    public string? Content { get; set; }
    public Guid? ParentId { get; set; }
    public int NumberLikeInComment { get; set; }
    public DateTime CreatedOn { get; set; }
    public UserDetailsDto? Owner { get; set; }
}
