using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Infrastructure.Class.GroupClasses;
public class GroupClassSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<GroupClassSeeder> _logger;


    public GroupClassSeeder(ISerializerService serializerService, ApplicationDbContext db, ILogger<GroupClassSeeder> logger)
    {
        _serializerService = serializerService;
        _db = db;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.GroupClasses.Any())
        {
            _logger.LogInformation("Started to Seed GroupClass.");

            // Here you can use your own logic to populate the database.
            // As an example, I am using a JSON file to populate the database.
            string groupClassData = await File.ReadAllTextAsync(path + "/Class/GroupClasses/groupClasses.json", cancellationToken);
            var groupClass = _serializerService.Deserialize<List<GroupClass>>(groupClassData);

            if (groupClass != null)
            {
                foreach (var gc in groupClass)
                {
                    await _db.GroupClasses.AddAsync(gc, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded GroupClasses.");
        }

        if (!_db.Classes.Any())
        {
            _logger.LogInformation("Started to Seed Classes.");

            string classesData = await File.ReadAllTextAsync(path + "/Class/classes.json", cancellationToken);
            var classes = _serializerService.Deserialize<List<Classes>>(classesData);

            if (classes != null)
            {
                foreach (var c in classes)
                {
                    await _db.Classes.AddAsync(c, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded TeacherPermissions.");
        }
    }
}
