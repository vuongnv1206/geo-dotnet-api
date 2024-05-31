using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.TeacherGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public interface INewReactionRepository : IScopedService
{
    Task AddNewsReaction(NewsReaction request);
    Task DeleteNewsReactionAsync(NewsReaction request);
    Task<NewsReaction> GetUserLikeTheNews(NewsReaction request);
}
