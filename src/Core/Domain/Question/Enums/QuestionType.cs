using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Question.Enums;
public enum QuestionType
{
    SingleChoice = 1,
    MultipleChoice = 2,
    FillBlank = 4,
    Matching = 5,
    Reading = 6,
    ReadingQuestionPassage = 7,
    Writing = 8,
    Other = 100
}
