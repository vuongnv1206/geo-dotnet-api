using FSH.WebApi.Domain.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Papers;
public class PaperPermissionSharedSpec :Specification<PaperPermission>, ISingleResultSpecification
{
    public PaperPermissionSharedSpec(Guid userId,Guid paperId)
    {
        Query.Where(x => x.UserId == userId && x.PaperId == paperId);
    }
}
