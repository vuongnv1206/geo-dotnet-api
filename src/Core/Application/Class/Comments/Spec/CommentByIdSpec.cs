using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.Comments.Spec;
public class CommentByIdSpec : Specification<Comment>, ISingleResultSpecification
{
    public CommentByIdSpec(Guid id)
    {
        Query
            .Include(x => x.CommentLikes)
            .Include(x => x.CommentChildrens)
            .Where(x => x.Id == id);
    }
}
