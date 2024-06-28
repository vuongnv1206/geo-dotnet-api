using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FSH.WebApi.Infrastructure.Class.News;
public class NewsReactionRepository : IPostLikeRepository
{

    private readonly ApplicationDbContext _context;

    public NewsReactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddNewsReaction(PostLike request)
    {
        _context.PostLike.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountLikeOfUserInNews(DefaultIdType classId)
    {
        var data = await _context.Post.Where(p => p.Id.Equals(classId)).ToListAsync();
        foreach (var item in data)
        {
            var count = await _context.PostLike.Where(p => p.PostId.Equals(item.Id)).ToListAsync();
            return count.Count();
        }
        return 0;
    }

    public async Task DeleteNewsReactionAsync(PostLike request)
    {
        _context.PostLike.Remove(request);
        await _context.SaveChangesAsync();
    }

    public async Task<PostLike> GetUserLikeTheNews(PostLike request) =>
        await _context.PostLike.FirstOrDefaultAsync(t => t.UserId == request.UserId && t.PostId == request.PostId);
}
