using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;

namespace FSH.WebApi.Infrastructure.TeacherGroup;
public class TeacherTeamSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<TeacherTeamSeeder> _logger;

    public TeacherTeamSeeder(ISerializerService serializerService, ApplicationDbContext db, ILogger<TeacherTeamSeeder> logger)
    {
        _serializerService = serializerService;
        _db = db;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        // string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        // if (!_db.TeacherTeams.Any())
        // {
        //    _logger.LogInformation("Started to Seed TeacherTeams.");
        //    string teacherTeamData = await File.ReadAllTextAsync(path + "/TeacherGroup/teacherTeam.json", cancellationToken);
        //    var teacherTeams = _serializerService.Deserialize<List<TeacherTeam>>(teacherTeamData);

        // if (teacherTeams != null)
        //    {
        //        foreach (var team in teacherTeams)
        //        {
        //            await _db.TeacherTeams.AddAsync(team, cancellationToken);
        //        }
        //    }

        // await _db.SaveChangesAsync(cancellationToken);
        //    _logger.LogInformation("Seeded TeacherTeams.");
        // }
    }
}
