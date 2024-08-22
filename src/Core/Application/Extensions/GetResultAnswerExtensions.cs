using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace FSH.WebApi.Application.Extensions;
public static class GetResultAnswerExtensions
{
    public static bool IsAnswerCorrect(this SubmitPaperDetail submitDetail, QuestionClone question, List<AnswerClone> correctAnswers)
    {
        if (string.IsNullOrEmpty(submitDetail.AnswerRaw))
        {
            return false;
        }

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

    public static float GetPointQuestion(this SubmitPaperDetail submitDetail, QuestionClone question, float mark)
    {
        return question.QuestionType switch
        {
            QuestionType.ReadingQuestionPassage or QuestionType.SingleChoice => CalculateSingleChoiceScore(submitDetail, question, mark),
            QuestionType.MultipleChoice => CalculateMultipleChoiceScore(submitDetail, question, mark),
            QuestionType.FillBlank => CalculateFillBlankScore(submitDetail, question, mark),
            QuestionType.Reading => CalculateReadingScore(submitDetail, question, mark),
            QuestionType.Matching => CalculateMatchingScore(submitDetail, question, mark),
            QuestionType.Writing => submitDetail.Mark ?? 0,
            QuestionType.Other => 0,
            _ => throw new NotImplementedException($"Question type {question.QuestionType} is not supported."),
        };
    }

    private static float CalculateReadingScore(SubmitPaperDetail submitDetail, QuestionClone question, float mark)
    {
        throw new NotImplementedException();
    }

    private static float CalculateSingleChoiceScore(SubmitPaperDetail submitDetail, QuestionClone question, float mark)
    {
        return Guid.TryParse(submitDetail.AnswerRaw, out var answerIdRaw) &&
            question.AnswerClones.SingleOrDefault(a => a.Id == answerIdRaw && a.IsCorrect) is not null
            ? mark
            : 0;
    }

    private static float CalculateMultipleChoiceScore(SubmitPaperDetail submitDetail, QuestionClone question, float mark)
    {
        var answerIds = submitDetail.AnswerRaw.Split('|', StringSplitOptions.RemoveEmptyEntries)
                                              .Select(Guid.Parse)
                                              .ToList();

        var correctAnswers = question.AnswerClones.Where(x => x.IsCorrect).ToList();
        if (correctAnswers.Count < answerIds.Count)
        {
            return 0;
        }

        float averageScore = mark / correctAnswers.Count;
        return answerIds.Intersect(correctAnswers.Select(a => a.Id)).Count() * averageScore;
    }

    private static float CalculateMatchingScore(SubmitPaperDetail submitDetail, QuestionClone question, float mark)
    {
        var matchingAnswers = submitDetail.AnswerRaw.Split('|')
                                                    .Select(ma => ma.Split('_'))
                                                    .ToDictionary(ma => ma[0], ma => ma[1]);

        var correctMatchings = question.AnswerClones[0].Content.Split('|')
                                                    .Select(ma => ma.Split('_'))
                                                    .ToDictionary(ma => ma[0], ma => ma[1]);

        float averageScore = mark / correctMatchings.Count;
        int numberCorrectAnswer = matchingAnswers.Count(raw => correctMatchings.TryGetValue(raw.Key, out string? correctValue) && correctValue == raw.Value);

        return numberCorrectAnswer * averageScore;
    }

    private static float CalculateFillBlankScore(SubmitPaperDetail submitDetail, QuestionClone question, float mark)
    {
        float achieveMark = 0;
        float averageScore = mark / question.AnswerClones.Count;
        var answerRaw = JArray.Parse(submitDetail.AnswerRaw);

        Regex regex = new Regex(@"\$_\[(\d+)\](.+)");
        foreach (var answer in question.AnswerClones)
        {
            Match match = regex.Match(answer.Content);

            if (match.Success)
            {
                string key = match.Groups[1].Value;
                string answerCorrent = match.Groups[2].Value;

                foreach(JObject obj in answerRaw)
                {
                    if (obj[key]?.ToString() == answerCorrent)
                    {
                        achieveMark += averageScore;
                    }
                }
            }
        }

        return achieveMark;
    }
}
