using Ardalis.Specification.EntityFrameworkCore;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.TeacherGroup;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FSH.WebApi.Infrastructure.TeacherGroup;
public class TeacherGroupSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<TeacherGroupSeeder> _logger;

    public TeacherGroupSeeder(ISerializerService serializerService, ApplicationDbContext db, ILogger<TeacherGroupSeeder> logger)
    {
        _serializerService = serializerService;
        _db = db;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.GroupTeachers.Any())
        {
            _logger.LogInformation("Started to Seed GroupTeacher.");
            string groupTeacherData = await File.ReadAllTextAsync(path + "/TeacherGroup/groupTeacher.json", cancellationToken);
            var groupTeachers = _serializerService.Deserialize<List<GroupTeacher>>(groupTeacherData);

            if (groupTeachers != null)
            {
                foreach (var group in groupTeachers)
                {
                    await _db.GroupTeachers.AddAsync(group, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded GroupTeachers.");
        }

        if (!_db.TeacherTeams.Any())
        {
            _logger.LogInformation("Started to Seed TeacherTeams.");
            string teacherTeamData = await File.ReadAllTextAsync(Path.Combine(path, "TeacherGroup/teacherTeam.json"), cancellationToken);
            var teacherTeams = _serializerService.Deserialize<List<TeacherTeam>>(teacherTeamData);

            if (teacherTeams != null)
            {
                foreach (var teacher in teacherTeams)
                {
                    await _db.TeacherTeams.AddAsync(teacher, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded TeacherTeams.");
        }

        if (!_db.TeacherInGroups.Any())
        {
            _logger.LogInformation("Started to Seed TeacherInGroups.");

            var teacherTeams = await _db.TeacherTeams.ToListAsync();
            var groupTeachers = await _db.GroupTeachers.ToListAsync();

            if (teacherTeams.Any() && groupTeachers.Any())
            {
                var teacherInGroups = new List<TeacherInGroup>();

                foreach (var groupTeacher in groupTeachers)
                {
                    var randomTeacherTeam = teacherTeams[new Random().Next(teacherTeams.Count)];

                    var teacherInGroup = new TeacherInGroup
                    {
                        TeacherTeamId = randomTeacherTeam.Id,
                        GroupTeacherId = groupTeacher.Id,
                    };

                    teacherInGroups.Add(teacherInGroup);
                }

                await _db.TeacherInGroups.AddRangeAsync(teacherInGroups, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Seeded TeacherInGroups.");
        }

    }
}

