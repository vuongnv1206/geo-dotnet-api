using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New.Spec;
public class PostByClassIdSpec : EntitiesByPaginationFilterSpec<Post, PostDto>
{
    public PostByClassIdSpec(GetPostRequest request)
        : base(request)
    {
        Query
            .Where(x => x.ClassesId == request.ClassId)
            .Include(x => x.Comments);
    }
}
