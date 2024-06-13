using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FSH.WebApi.Infrastructure.Examination;

public class PaperSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PaperSeeder> _logger;

    public PaperSeeder(ISerializerService serializerService, ILogger<PaperSeeder> logger, ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!_db.Papers.Any())
        {
            _logger.LogInformation("Started to Seed Papers.");

            string paperData = await File.ReadAllTextAsync(Path.Combine(path, "Examination", "paper.json"), cancellationToken);
            var papers = _serializerService.Deserialize<List<Paper>>(paperData);

            if (papers != null)
            {
                // Lấy PaperLabel và PaperFolder từ cơ sở dữ liệu
                var paperLabels = await _db.PaperLabels.ToListAsync(cancellationToken);
                var paperFolders = await _db.PaperFolders.ToListAsync(cancellationToken);
                var questions = await _db.Questions.ToListAsync(cancellationToken);

                foreach (var paper in papers)
                {

                    paper.PaperLabelId = paperLabels.FirstOrDefault()?.Id;
                    paper.PaperFolderId = paperFolders.FirstOrDefault()?.Id;
                    paper.PaperQuestions = questions.Take(2).Select(q => new PaperQuestion 
                    {
                        QuestionId = q.Id,
                        Mark = 5 // Giá trị Mark mẫu
                    }).ToList();
                    await _db.Papers.AddAsync(paper, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded Papers.");
        }
    }
}
