namespace FSH.WebApi.Application.Subjects;
public class DeleteRandomSubjectRequest : IRequest<string>
{
}

public class DeleteRandomSubjectRequestHandler : IRequestHandler<DeleteRandomSubjectRequest, string>
{
    private readonly IJobService _jobService;

    public DeleteRandomSubjectRequestHandler(IJobService jobService) => _jobService = jobService;

    public Task<string> Handle(DeleteRandomSubjectRequest request, CancellationToken cancellationToken)
    {
        string jobId = _jobService.Schedule<ISubjectGeneratorJob>(x => x.CleanAsync(default), TimeSpan.FromSeconds(5));
        return Task.FromResult(jobId);
    }
}