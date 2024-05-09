using FSH.WebApi.Application.Examination.PaperFolders.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class GetPaperFolderRequest : IRequest<PaperFolderDto>
{
    public Guid? ParentId { get; }
    public GetPaperFolderRequest(Guid? parentId)
    {
        ParentId = parentId;
    }
}
