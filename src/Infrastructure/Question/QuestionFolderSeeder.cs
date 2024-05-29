using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FSH.WebApi.Infrastructure.Question;

public class QuestionFolderSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<QuestionFolderSeeder> _logger;

    public QuestionFolderSeeder(ISerializerService serializerService, ILogger<QuestionFolderSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var adminUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@root.com", cancellationToken);
        var basicUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == "basic@root.com", cancellationToken);
        var adminGuid = Guid.Parse(adminUser.Id);
        var basicGuid = Guid.Parse(basicUser.Id);

        if (!_db.QuestionFolders.Any())
        {
            _logger.LogInformation("Started to Seed Question Folders.");

            // Here you can use your own logic to populate the database.
            // As an example, I am using a JSON file to populate the database.
            string questionFolderData = await File.ReadAllTextAsync(path + "/Question/QuestionFolder.json", cancellationToken);
            var questionFolders = _serializerService.Deserialize<List<QuestionFolder>>(questionFolderData);

            if (questionFolders != null)
            {
                foreach (var questionFolder in questionFolders)
                {
                    if (questionFolder.ParentId == null && adminUser != null)
                    {
                        await _db.QuestionFolders.AddAsync(questionFolder, cancellationToken);

                        QuestionFolder child = new QuestionFolder("Child 1", null);
                        await _db.QuestionFolders.AddAsync(child, cancellationToken);

                        QuestionFolder child2 = new QuestionFolder("Child 2", null);
                        await _db.QuestionFolders.AddAsync(child2, cancellationToken);

                        questionFolder.AddChild(child);
                        child.AddChild(child2);
                    }
                }

                await _db.SaveChangesAsync(cancellationToken);
            }

            // add creator for all folders in the database
            var folders = await _db.QuestionFolders.ToListAsync(cancellationToken);
            foreach (var folder in folders)
            {
                folder.CreatedBy = adminGuid;
            }

            _logger.LogInformation("Seeded Question Folder.");
        }

        // seed question folder permissions
        if (!_db.QuestionFolderPermissions.Any())
        {
            _logger.LogInformation("Started to Seed Question Folder Permissions.");

            var folders = await _db.QuestionFolders.ToListAsync(cancellationToken);

            foreach (var folder in folders)
            {
                await _db.QuestionFolderPermissions.AddAsync(new QuestionFolderPermission(basicGuid, Guid.Empty, folder.Id, true, true, true, true), cancellationToken);
                await _db.QuestionFolderPermissions.AddAsync(new QuestionFolderPermission(adminGuid, Guid.Empty, folder.Id, true, true, true, true), cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);

            // add creator for all permissions in the database
            var permissions = await _db.QuestionFolderPermissions.ToListAsync(cancellationToken);
            foreach (var permission in permissions)
            {
                permission.CreatedBy = adminGuid;
                permission.LastModifiedBy = adminGuid;
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Seeded Question Folder Permissions.");
        }
    }
}