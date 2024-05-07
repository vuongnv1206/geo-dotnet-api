using Ardalis.Specification.EntityFrameworkCore;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.TeacherGroup;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using MassTransit;
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


        if (!_db.TeacherInGroups.Any())
        {
            _logger.LogInformation("Started to Seed TeacherInGroups.");

            var groupTeachers = await _db.GroupTeachers.ToListAsync();

            if (groupTeachers.Any())
            {
                var teacherInGroups = groupTeachers.Select(groupTeacher => new TeacherInGroup
                {
                    TeacherId = Guid.NewGuid(),
                    GroupTeacherId = groupTeacher.Id,
                }).ToList();

                await _db.TeacherInGroups.AddRangeAsync(teacherInGroups, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("Seeded TeacherInGroups.");
        }

    }
}

