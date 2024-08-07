using FSH.WebApi.Domain.Question.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions.Specs;
public class QuestionFromMatrixSpec : Specification<Domain.Question.Question>
{
    public QuestionFromMatrixSpec(List<Guid> questionFolderIds,Guid questionLabelId,QuestionType questionType)
    {
        Query
            .Where(x => x.QuestionFolderId.HasValue
                    && questionFolderIds.Contains(x.QuestionFolderId.Value)
                    && x.QuestionLableId == questionLabelId
                    && x.QuestionType == questionType);
    }
}
