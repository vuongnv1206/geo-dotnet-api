using System;
using FSH.WebApi.Domain.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Class;
public class NewsReaction
{
    public Guid UserId { get; private set; }
    public Guid NewsId { get; private set; }
    public virtual News News { get; private set; }

    public NewsReaction(Guid userId, Guid newsId)
    {
        UserId = userId;
        NewsId = newsId;
    }
}
