using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New;
public interface IPostLikeRepository : IScopedService
{
    Task AddNewsReaction(PostLike request);
    Task DeleteNewsReactionAsync(PostLike request);
    Task<PostLike> GetUserLikeTheNews(PostLike request);
    Task<int> CountLikeOfUserInNews(Guid classId);
}
