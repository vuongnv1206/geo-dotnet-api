﻿
using FSH.WebApi.Application.Questions.Dtos;



namespace FSH.WebApi.Application.Examination.Services.Models;
public class QuestionModel
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public int? RawIndex { get; set; }
    public float? Mark { get; set; }
    public string? QuestionType { get; set; }
    public QuestionLableDto? QuestionLabel { get; set; }
    public List<QuestionPassageModel>? QuestionPassages { get; set; }
    public List<AnswerModel>? Answers { get; set; } = new();
}
