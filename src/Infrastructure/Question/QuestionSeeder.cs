using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FSH.WebApi.Infrastructure.Question;
public class QuestionSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<QuestionSeeder> _logger;
    private Guid _adminGuid;

    public QuestionSeeder(ISerializerService serializerService, ILogger<QuestionSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var adminUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@root.com", cancellationToken);
        _adminGuid = Guid.Parse(adminUser.Id);

        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.Questions.Any())
        {
            _logger.LogInformation("Started to Seed Questions.");

            string questionData = await File.ReadAllTextAsync(path + "/Question/Question.json", cancellationToken);
            var questions = _serializerService.Deserialize<List<Domain.Question.Question>>(questionData);

            if (questions != null)
            {
                foreach (var question in questions)
                {
                    question.QuestionStatus = QuestionStatus.Approved;
                    await SeedQuestion(question, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded Questions.");
        }
    }

    private async Task SeedQuestion(Domain.Question.Question question, CancellationToken cancellationToken)
    {

        if (question.QuestionFolder != null)
        {
            var questionFolder = await _db.QuestionFolders.FirstOrDefaultAsync(x => x.Id == question.QuestionFolderId || x.Name == question.QuestionFolder.Name, cancellationToken);
            if (questionFolder == null)
            {
                questionFolder = new QuestionFolder(question.QuestionFolder.Name, null);
                await _db.QuestionFolders.AddAsync(questionFolder, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                question.QuestionFolder = questionFolder;
            }
            else
            {
                question.QuestionFolder = questionFolder;
            }

            // add creator for all folders
            var folders = await _db.QuestionFolders.ToListAsync(cancellationToken);
            foreach (var folder in folders)
            {
                folder.CreatedBy = _adminGuid;
            }

            await _db.SaveChangesAsync(cancellationToken);

            // seed question folder permissions
            foreach (var folder in folders)
            {
                var permission = new QuestionFolderPermission(_adminGuid, Guid.Empty, folder.Id, true, true, true, true, true);

                // check if permission already exists
                var existingPermission = await _db.QuestionFolderPermissions.FirstOrDefaultAsync(x => x.UserId == _adminGuid && x.QuestionFolderId == folder.Id, cancellationToken);
                if (existingPermission == null)
                {
                    await _db.QuestionFolderPermissions.AddAsync(permission, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

        }

        question.CreatedBy = _adminGuid;
        await _db.Questions.AddAsync(question, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }
}

