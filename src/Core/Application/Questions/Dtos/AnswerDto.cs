﻿using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions;

public class AnswerDto : IDto
{
    public Guid? Id { get; set; }
    public string? Content { get; set; }
    public Guid? QuestionId { get; set; }
    public bool IsCorrect { get; set; }
}