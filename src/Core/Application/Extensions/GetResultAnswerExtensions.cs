using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;

namespace FSH.WebApi.Application.Extensions;
public static class GetResultAnswerExtensions
{
    public static bool IsAnswerCorrect(this SubmitPaperDetail submitDetail, Question question, List<Answer> correctAnswers)
    {
        switch (question.QuestionType)
        {
            case QuestionType.SingleChoice:
                return correctAnswers.Any(a => a.Id == Guid.Parse(submitDetail.AnswerRaw) && a.IsCorrect);

            case QuestionType.MultipleChoice:
                var answerIds = submitDetail.AnswerRaw.Split('|').Select(Guid.Parse).ToList();
                var correctAnswerIds = correctAnswers.Where(a => a.IsCorrect).Select(a => a.Id).ToList();
                return !correctAnswerIds.Except(answerIds).Any() && !answerIds.Except(correctAnswerIds).Any();

            case QuestionType.FillBlank:
                return false;

            case QuestionType.Matching:
                var matchingAnswers = submitDetail.AnswerRaw.Split('|').Select(ma => ma.Split('_')).ToDictionary(ma => ma[0], ma => ma[1]);
                var correctMatchings = correctAnswers.ToDictionary(a => a.Id.ToString(), a => a.Content);
                return matchingAnswers.All(ma => correctMatchings.ContainsKey(ma.Key) && correctMatchings[ma.Key] == ma.Value);

            case QuestionType.Writing:
                return false;
            case QuestionType.Other:
                return false;
            case QuestionType.ReadingQuestionPassage:
                return false;
            case QuestionType.Reading:
                return false;

            default:
                throw new NotImplementedException($"Question type {question.QuestionType} is not supported.");
        }
    }
}
