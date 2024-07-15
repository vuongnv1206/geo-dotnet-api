using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.Services;
public interface IPaperTemplateService : ITransientService
{
    string GeneratePaperTemplate<T>(string templateName, T paperTemplateModel);
}
