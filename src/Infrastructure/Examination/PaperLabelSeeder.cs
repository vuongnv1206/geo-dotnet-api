using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.Extensions.Logging;
using System.Reflection;


namespace FSH.WebApi.Infrastructure.Examination;
public class PaperLabelSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PaperLabelSeeder> _logger;

    public PaperLabelSeeder(ISerializerService serializerService, ApplicationDbContext db, ILogger<PaperLabelSeeder> logger)
    {
        _serializerService = serializerService;
        _db = db;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.PaperLabels.Any())
        {
            _logger.LogInformation("Started to Seed PaperLabels.");

            string paperLabelData = await File.ReadAllTextAsync(path + "/Examination/paperLabel.json", cancellationToken);
            var paperLabels = _serializerService.Deserialize<List<PaperLabel>>(paperLabelData);

            await _db.PaperLabels.AddRangeAsync(paperLabels, cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded PaperLabels.");
        }
    }

}
