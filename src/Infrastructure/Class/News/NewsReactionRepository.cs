using FSH.WebApi.Application.Class.New;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Infrastructure.Class.News;
public class NewsReactionRepository : INewReactionRepository
{

    private readonly ApplicationDbContext _context;

    public NewsReactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddNewsReaction(NewsReaction request)
    {
        _context.NewsReactions.Add(request);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNewsReactionAsync(NewsReaction request)
    {
        _context.NewsReactions.Remove(request);
        await _context.SaveChangesAsync();
    }

    public async Task<NewsReaction> GetUserLikeTheNews(NewsReaction request) =>
        await _context.NewsReactions.FirstOrDefaultAsync(t => t.UserId == request.UserId && t.NewsId == request.NewsId);
}
