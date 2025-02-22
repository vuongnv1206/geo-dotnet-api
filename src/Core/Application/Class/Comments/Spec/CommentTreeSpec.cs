﻿using FSH.WebApi.Domain.Class;

namespace FSH.WebApi.Application.Class.Comments.Spec;
public class CommentTreeSpec : Specification<Comment>
{
    public CommentTreeSpec()
    {
        Query.Include(x => x.CommentLikes).Include(x => x.CommentParent).Include(x => x.CommentChildrens);
    }
}
