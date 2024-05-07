using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Subjects;
public class GenerateRandomSubjectRequest : IRequest<string>
{
    public int NSeed { get; set; }
}

public class GenerateRandomSubjectRequestHandler : IRequestHandler<GenerateRandomSubjectRequest, string>
{
    private readonly IJobService _jobService;

    public GenerateRandomSubjectRequestHandler(IJobService jobService) => _jobService = jobService;

    public Task<string> Handle(GenerateRandomSubjectRequest request, CancellationToken cancellationToken)
    {
        string jobId = _jobService.Enqueue<ISubjectGeneratorJob>(x => x.GenerateAsync(request.NSeed, default));
        return Task.FromResult(jobId);
    }
}