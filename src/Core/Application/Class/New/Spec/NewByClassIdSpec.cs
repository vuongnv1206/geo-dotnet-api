using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Spec;
public class NewByClassIdSpec : EntitiesByPaginationFilterSpec<News, NewsDto>
{
    public NewByClassIdSpec(GetNewsRequest request) : base(request)

        => Query.Include(x => x.Classes);
}
