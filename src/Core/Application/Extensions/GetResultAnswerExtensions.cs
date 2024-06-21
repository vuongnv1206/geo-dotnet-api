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

            case QuestionType.ReadingQuestionPassage:
            case QuestionType.Writing:
            case QuestionType.Reading:
            case QuestionType.Other:
                return false;

            default:
                throw new NotImplementedException($"Question type {question.QuestionType} is not supported.");
        }
    }

    public static float GetPointQuestion(this SubmitPaperDetail submitDetail, Question question, float mark)
    {
        switch (question.QuestionType)
        {
            case QuestionType.ReadingQuestionPassage:
            case QuestionType.SingleChoice:
                return CalculateSingleChoiceScore(submitDetail, question, mark);
            case QuestionType.MultipleChoice:
                return CalculateMultipleChoiceScore(submitDetail, question, mark);
            case QuestionType.FillBlank:
                return CalculateFillBlankScore(submitDetail, question, mark);
            case QuestionType.Reading:
                return CalculateReadingScore(submitDetail, question, mark);
            case QuestionType.Matching:
                return CalculateMatchingScore(submitDetail, question, mark);
            case QuestionType.Writing:
            case QuestionType.Other:
                return 0;
            default:
                throw new NotImplementedException($"Question type {question.QuestionType} is not supported.");
        }
    }

    private static float CalculateReadingScore(SubmitPaperDetail submitDetail, Question question, float mark)
    {
        throw new NotImplementedException();
    }

    private static float CalculateSingleChoiceScore(SubmitPaperDetail submitDetail, Question question, float mark)
    {
        if (Guid.TryParse(submitDetail.AnswerRaw, out var answerIdRaw) &&
            question.Answers.SingleOrDefault(a => a.Id == answerIdRaw && a.IsCorrect) is not null)
        {
            return mark;
        }
        return 0;
    }

    private static float CalculateMultipleChoiceScore(SubmitPaperDetail submitDetail, Question question, float mark)
    {
        var answerIds = submitDetail.AnswerRaw.Split('|', StringSplitOptions.RemoveEmptyEntries)
                                              .Select(Guid.Parse)
                                              .ToList();

        var correctAnswers = question.Answers.Where(x => x.IsCorrect).ToList();
        if (correctAnswers.Count < answerIds.Count)
        {
            return 0;
        }

        float averageScore = mark / correctAnswers.Count;
        return answerIds.Intersect(correctAnswers.Select(a => a.Id)).Count() * averageScore;
    }

    private static float CalculateMatchingScore(SubmitPaperDetail submitDetail, Question question, float mark)
    {
        var matchingAnswers = submitDetail.AnswerRaw.Split('|')
                                                    .Select(ma => ma.Split('_'))
                                                    .ToDictionary(ma => ma[0], ma => ma[1]);

        var correctMatchings = question.Answers[0].Content.Split('|')
                                                    .Select(ma => ma.Split('_'))
                                                    .ToDictionary(ma => ma[0], ma => ma[1]);

        float averageScore = mark / correctMatchings.Count;
        int numberCorrectAnswer = matchingAnswers.Count(raw => correctMatchings.TryGetValue(raw.Key, out var correctValue) && correctValue == raw.Value);

        return numberCorrectAnswer * averageScore;
    }

    private static float CalculateFillBlankScore(SubmitPaperDetail submitDetail, Question question, float mark)
    {
        return 0;
    }
}
