using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFolderTreeSpec : Specification<QuestionFolder>
{
    public QuestionFolderTreeSpec()
    {
        Query
      .Include(b => b.Permissions)
      .Include(b => b.Children)
      .Include(x => x.Questions).ThenInclude(x => x.QuestionLable);
    }
}
