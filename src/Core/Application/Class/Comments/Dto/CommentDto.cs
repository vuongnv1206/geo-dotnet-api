using FSH.WebApi.Application.Identity.Users;

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
