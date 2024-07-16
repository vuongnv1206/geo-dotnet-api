using FSH.WebApi.Application.Class.Comments.Dto;
using FSH.WebApi.Application.Identity.Users;

namespace FSH.WebApi.Application.Class.New.Dto;
public class PostDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ClassesId { get; set; }
    public DefaultIdType CreatedBy { get; set; }
    public string? Content { get; set; }
    public bool IsLockComment { get; set; }
    public DateTime CreatedOn { get; set; }
    public int? NumberLikeInThePost { get; set; }
    public List<CommentDto>? Comments { get; set; }
    public UserDetailsDto? Owner { get; set; }
}
