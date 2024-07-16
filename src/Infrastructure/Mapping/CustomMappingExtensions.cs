using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using Mapster;

namespace FSH.WebApi.Infrastructure.Mapping;
public static class CustomMappingExtensions
{
    public static List<QuestionDto> MapQuestions(List<PaperQuestion> paperQuestions)
    {
        var questionDtos = paperQuestions.Select(pq => pq.Question.Adapt<QuestionDto>()).ToList();

        foreach (var pq in paperQuestions)
        {
            var questionDto = questionDtos.FirstOrDefault(q => q.Id == pq.QuestionId);
            if (questionDto != null)
            {
                questionDto.RawIndex = pq.RawIndex;
                questionDto.Mark = pq.Mark;
            }
        }

        return questionDtos;
    }

    public static List<QuestionForStudentDto> MapQuestionsForStudent(List<PaperQuestion> paperQuestions)
    {
        var questionDtos = paperQuestions.Select(pq => pq.Question.Adapt<QuestionForStudentDto>()).ToList();

        foreach (var pq in paperQuestions)
        {
            var questionDto = questionDtos.FirstOrDefault(q => q.Id == pq.QuestionId);
            if (questionDto != null)
            {
                questionDto.RawIndex = pq.RawIndex;
                questionDto.Mark = pq.Mark;

                // Map answers for Question passages
                if (pq.Question.QuestionType == QuestionType.Reading)
                {
                    foreach (var qp in pq.Question.QuestionPassages)
                    {
                        var questionPassageDto = questionDto.QuestionPassages.FirstOrDefault(q => q.Id == qp.Id);
                        if (questionPassageDto != null)
                        {
                            questionPassageDto.Answers = MapAnswersForStudent(qp.AnswerClones);
                        }
                    }
                }

            }
        }

        return questionDtos;
    }

    public static List<AnswerForStudentDto> MapAnswers(List<Answer> answers)
    {
        foreach (var answer in answers)
        {
            answer.IsCorrect = false;
        }

        return answers.Select(a => a.Adapt<AnswerForStudentDto>()).ToList();
    }

    public static List<AnswerForStudentDto> MapAnswersForStudent(List<AnswerClone> answers)
    {
        return answers.Select(a => a.Adapt<AnswerForStudentDto>()).ToList();
    }
}
