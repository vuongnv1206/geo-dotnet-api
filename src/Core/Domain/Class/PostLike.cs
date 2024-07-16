namespace FSH.WebApi.Domain.Class;
public class PostLike
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
    public virtual Post Post { get; set; }
    public PostLike(Guid userId, Guid postId)
    {
        UserId = userId;
        PostId = postId;
    }
}
