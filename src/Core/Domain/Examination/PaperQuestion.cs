﻿namespace FSH.WebApi.Domain.Examination;
public class PaperQuestion
{
    public Guid PaperId { get; set; }
    public Guid QuestionId { get; set; }
    public virtual Paper Paper { get; set; }
    public virtual Question.Question Question { get; set; }
}
