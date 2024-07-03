using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.New.Spec;
public class PostByIdSpec : Specification<Post>, ISingleResultSpecification
{
    public PostByIdSpec(DefaultIdType id)
    {
        Query.Where(x => x.Id == id).Include(x => x.PostLikes).Include(x => x.Comments);
    }
}
