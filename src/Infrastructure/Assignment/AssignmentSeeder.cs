using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;
using System.Reflection;
using FSH.WebApi.Domain.Assignment;

namespace FSH.WebApi.Infrastructure.Assignments;
public class AssignmentSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AssignmentSeeder> _logger;

    public AssignmentSeeder(ISerializerService serializerService, ILogger<AssignmentSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var subjectIds = _db.Subjects.Select(x => x.Id).ToList();
        if (!_db.Assignments.Any())
        {
            _logger.LogInformation("Started to Seed Assignments.");

            // Here you can use your own logic to populate the database.
            // As an example, I am using a JSON file to populate the database.
            string assignmentData = await File.ReadAllTextAsync(path + "/Assignment/assignment.json", cancellationToken);
            var assignments = _serializerService.Deserialize<List<Assignment>>(assignmentData);

            if (assignments != null)
            {
                foreach (var assignment in assignments)
                {
                    assignment.SubjectId = subjectIds[new Random().Next(0, subjectIds.Count)];
                    await _db.Assignments.AddAsync(assignment, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded Assignments.");
        }
    }
}