using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Host.Controllers.Examination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FSH.WebApi.Application.Examination.SubmitPapers;
public interface ISubmmitPaperService : ITransientService
{
    public Task<PaperForStudentDto> StartExamAsync(StartExamRequest request, CancellationToken cancellationToken);

}
