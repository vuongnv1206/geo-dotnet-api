﻿using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;
public class QuestionFoldersWithPermissionsSpecByUserId : Specification<QuestionFolder>, ISingleResultSpecification
{
    public QuestionFoldersWithPermissionsSpecByUserId(Guid userId, Guid? parentFolderId) =>
        Query
        .Include(qf => qf.Permissions)
        .Include(qf => qf.Children)
        .Where(qf => qf.Permissions.Any(qfp => qfp.UserId == userId && qfp.CanView) || qf.CreatedBy == userId)
        .Where(qf => qf.ParentId == parentFolderId)
        .OrderBy(qf => qf.Name);
}