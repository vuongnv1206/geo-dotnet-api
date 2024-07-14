﻿using FSH.WebApi.Domain.Examination;
using MediatR;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public class SubmitPaperByPaperIdPaging : EntitiesByPaginationFilterSpec<SubmitPaper, SubmitPaperDto>
{
    public SubmitPaperByPaperIdPaging(GetSubmittedPaperRequest request, Paper paper, Guid userId)
        : base(request)
    {
        Query
            .Where(x => x.PaperId == paper.Id
            && ( x.CreatedBy == userId || paper.CreatedBy == userId))
            .Include(sb => sb.Paper)
                .ThenInclude(p => p.PaperAccesses)
                .Where(sb => !request.ClassId.HasValue || sb.Paper.PaperAccesses.Any(x => x.ClassId == request.ClassId));
    }
}
