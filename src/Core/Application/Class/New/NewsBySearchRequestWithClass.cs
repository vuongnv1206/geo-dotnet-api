using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class NewsBySearchRequestWithClass : EntitiesByPaginationFilterSpec<News, NewsDto>
{
    public NewsBySearchRequestWithClass(GetNewsRequest request)
        : base(request)
    {
        Query
            .Include(p => p.Classes)
            .OrderBy(c => c.CreatedOn, !request.HasOrderBy())
            .Where(p => p.ClassesId.Equals(request.ClassesId!.Value), request.ClassesId.HasValue);
    }
}
