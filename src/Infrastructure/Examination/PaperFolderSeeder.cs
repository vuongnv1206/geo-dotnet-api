using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
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
                foreach (var paperFolder in paperFolders)
                {
                    await SeedPaperFolder(paperFolder, null, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded PaperFolders.");
        }
    }

    private async Task SeedPaperFolder(PaperFolder paperFolder, DefaultIdType? parentId, CancellationToken cancellationToken)
    {
        if (parentId.HasValue)
        {
            paperFolder.ParentId = parentId;
        }

        await _db.PaperFolders.AddAsync(paperFolder, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

    }
}
