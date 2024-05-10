using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperLabels;
public class SearchPaperLabelRequest : PaginationFilter, IRequest<PaginationResponse<PaperLabelDto>>
{
}
