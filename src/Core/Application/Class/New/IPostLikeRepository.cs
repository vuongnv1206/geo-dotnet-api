using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public interface IPostLikeRepository : IScopedService
{
    Task AddNewsReaction(PostLike request);
    Task DeleteNewsReactionAsync(PostLike request);
    Task<PostLike> GetUserLikeTheNews(PostLike request);
    Task<int> CountLikeOfUserInNews(Guid classId);
}
