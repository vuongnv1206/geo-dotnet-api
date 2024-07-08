using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
        }

        return questionDtos;
    }
}
