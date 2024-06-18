using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FSH.WebApi.Infrastructure.Examination;
public class PaperFolderSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PaperFolderSeeder> _logger;

    public PaperFolderSeeder(ISerializerService serializerService, ILogger<PaperFolderSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.PaperFolders.Any())
        {
            _logger.LogInformation("Started to Seed PaperFolders.");

            string paperFolderData = await File.ReadAllTextAsync(path + "/Examination/paperFolder.json", cancellationToken);
            var paperFolders = _serializerService.Deserialize<List<PaperFolder>>(paperFolderData);

            if (paperFolders != null)
            {
                var adminUser = _db.Users.FirstOrDefault(u => u.Email == "admin@root.com");
                
                foreach (var paperFolder in paperFolders)
                {
                    if (adminUser != null)
                    {
                        paperFolder.CreatedBy = Guid.Parse(adminUser.Id);
                    }
                    else
                    {
                        // Xử lý trường hợp không tìm thấy admin user
                        _logger.LogWarning("Admin user not found. PaperFolder CreatedBy not set.");
                    }
                    await _db.PaperFolders.AddAsync(paperFolder, cancellationToken);
                    _db.SaveChanges();
                }

                var paperFoldersSeeded = await _db.PaperFolders.ToListAsync(cancellationToken);
                //update creadted by for all paperFoldersSeeded
                paperFoldersSeeded.ForEach(p => p.CreatedBy = Guid.Parse(adminUser.Id));
                _db.SaveChanges();

            }


            _logger.LogInformation("Seeded PaperFolders.");
        }
    }

}
