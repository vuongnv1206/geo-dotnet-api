using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;
using System.Reflection;
using FSH.WebApi.Domain.Subjects;

namespace FSH.WebApi.Infrastructure.Subjects;
public class SubjectSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SubjectSeeder> _logger;

    public SubjectSeeder(ISerializerService serializerService, ILogger<SubjectSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.Subjects.Any())
        {
            _logger.LogInformation("Started to Seed Subjects.");

            // Here you can use your own logic to populate the database.
            // As an example, I am using a JSON file to populate the database.
            string subjectData = await File.ReadAllTextAsync(path + "/Subjects/subject.json", cancellationToken);
            var subjects = _serializerService.Deserialize<List<Subject>>(subjectData);

            if (subjects != null)
            {
                foreach (var subject in subjects)
                {
                    await _db.Subjects.AddAsync(subject, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded Subjects.");
        }
    }
}