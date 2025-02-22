﻿using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.Reviews;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public interface ISubmmitPaperService : ITransientService
{
    Task<PaperForStudentDto> StartExamAsync(StartExamRequest request, CancellationToken cancellationToken);
    Task<string> SubmitExamAsync(SubmitExamRequest request, CancellationToken cancellationToken);
    string FormatAnswerRaw(SubmitPaperQuestion spq, QuestionClone question);

    Task<float> CalculateScoreSubmitPaper(Guid submitPaperId);
    Task<List<SubmitPaper>> CalculateScorePaperAsync(Guid paperId, CancellationToken cancellationToken);
    Task<DefaultIdType> SendLogAsync(SendLogRequest request, CancellationToken cancellationToken);
    bool IsCorrectAnswer(SubmitPaperDetail submitDetail, QuestionClone question, List<SubmitPaperDetail>? details = null);
    Task<LastResultExamDto> GetLastResultExamAsync(Guid paperId, Guid userId, Guid submitPaperId, CancellationToken cancellationToken);
}
