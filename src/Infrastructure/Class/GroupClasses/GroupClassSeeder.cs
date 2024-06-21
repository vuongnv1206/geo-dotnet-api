using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

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

        var adminUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@root.com", cancellationToken);
        var basicUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == "basic@root.com", cancellationToken);
        var adminGuid = Guid.Parse(adminUser.Id);
        var basicGuid = Guid.Parse(basicUser.Id);

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

            var groupClasses = await _db.GroupClasses.ToListAsync(cancellationToken);
            foreach (var g in groupClasses)
            {
                g.CreatedBy = adminGuid;
            }

            _logger.LogInformation("Seeded GroupClasses.");
        }

        if (!_db.Classes.Any())
        {
            _logger.LogInformation("Started to Seed Classes.");

            var groupClasses = await _db.GroupClasses.ToListAsync(cancellationToken);

            foreach (var gc in groupClasses)
            {
                await _db.Classes.AddAsync(new Classes("Se1600", "2024", basicGuid, gc.Id));
            }

            await _db.SaveChangesAsync(cancellationToken);
            // add creator for all classes in the database
            var classes = await _db.Classes.ToListAsync(cancellationToken);
            foreach (var c in classes)
            {
                c.CreatedBy = adminGuid;
            }


            _logger.LogInformation("Seeded Class.");
        }

        if (!_db.News.Any())
        {
            _logger.LogInformation("Started to Seed News.");

            var classes = await _db.Classes.ToListAsync(cancellationToken);

            foreach (var c in classes)
            {
                await _db.News.AddAsync(new Domain.Class.News("The news in the class", true, null, c.Id));
            }

            await _db.SaveChangesAsync(cancellationToken);

            // add creator for all news in the database
            var news = await _db.News.ToListAsync(cancellationToken);
            foreach (var c in news)
            {
                c.CreatedBy = adminGuid;
            }

            _logger.LogInformation("Seeded News.");
        }

        if (!_db.NewsReactions.Any())
        {
            _logger.LogInformation("Started to Seed NewsReaction.");

            var news = await _db.News.ToListAsync(cancellationToken);

            foreach (var n in news)
            {
                await _db.NewsReactions.AddAsync(new NewsReaction(adminGuid, n.Id));
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded NewsReaction.");
        }
    }
}
