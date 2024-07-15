using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Host.Controllers.Examination;

namespace FSH.WebApi.Application.Examination.SubmitPapers;
public interface ISubmmitPaperService : ITransientService
{
    public Task<PaperForStudentDto> StartExamAsync(StartExamRequest request, CancellationToken cancellationToken);
    Task<DefaultIdType> SubmitExamAsync(SubmitExamRequest request, CancellationToken cancellationToken);
    string FormatAnswerRaw(SubmitPaperQuestion spq, QuestionClone question);
}
