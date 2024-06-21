using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question.Enums;
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
                    paper.PaperQuestions = questions.OrderBy(x => Guid.NewGuid()).Take(5).Select(q => new PaperQuestion
                    {
                        QuestionId = q.Id,
                        Mark = 5 // Giá trị Mark mẫu
                    }).ToList();
                    paper.CreatedBy = Guid.Parse(_db.Users.FirstOrDefault(u => u.Email == "admin@root.com").Id);
                    await _db.Papers.AddAsync(paper, cancellationToken);
                }
            }

            _db.SaveChanges();
            _logger.LogInformation("Seeded Papers.");
        }

        if (!_db.SubmitPapers.Any())
        {
            _logger.LogInformation("Started to Seed Submit Papers.");
            var paperIds = _db.Papers.Select(x => x.Id).ToList();

            string submitPaperData = await File.ReadAllTextAsync(Path.Combine(path, "Examination", "submitPaper.json"), cancellationToken);
            var submitPapers = _serializerService.Deserialize<List<SubmitPaper>>(submitPaperData);

            if (submitPapers != null)
            {
                foreach (var submitPaper in submitPapers)
                {
                    if (!paperIds.Contains(submitPaper.PaperId))
                    {
                        var random = new Random();
                        submitPaper.PaperId = paperIds[random.Next(paperIds.Count)];

                    }

                    submitPaper.CreatedBy = Guid.Parse(_db.Users.FirstOrDefault(u => u.Email == "admin@root.com").Id);


                    await _db.SubmitPapers.AddAsync(submitPaper, cancellationToken);
                }
            }

            _db.SaveChanges();
            _logger.LogInformation("Seeded Submit Papers.");
        }


        if (!_db.SubmitPaperDetails.Any())
        {
            _logger.LogInformation("Seeding SubmitPaperDetails...");

            var submitPapers = await _db.SubmitPapers.Include(sp => sp.Paper).ToListAsync(cancellationToken);
            // Assuming PaperQuestions is the entity that links Papers to Questions
            var paperQuestions = await _db.PaperQuestions.Include(pq => pq.Question).ThenInclude(q => q.Answers).ToListAsync(cancellationToken);

            var submitPaperDetails = new List<SubmitPaperDetail>();

            foreach (var submitPaper in submitPapers)
            {
                // Filter PaperQuestions to get only those associated with this submitPaper's Paper
                var questionsForThisPaper = paperQuestions.Where(pq => pq.PaperId == submitPaper.PaperId)
                                                           .Select(pq => pq.Question)
                                                           .ToList();

                foreach (var question in questionsForThisPaper)
                {
                    string answerRaw = GenerateAnswerRawForQuestionType(question);

                    var submitPaperDetail = new SubmitPaperDetail(submitPaper.Id, question.Id, answerRaw);
                    submitPaperDetails.Add(submitPaperDetail);
                }
            }

            await _db.SubmitPaperDetails.AddRangeAsync(submitPaperDetails, cancellationToken);
            _db.SaveChanges();

            _logger.LogInformation("Seeded SubmitPaperDetails.");
        }


    }



    private string GenerateAnswerRawForQuestionType(Domain.Question.Question question)
    {
        switch (question.QuestionType)
        {
            case QuestionType.SingleChoice:
                return question.Answers.FirstOrDefault()?.Id.ToString() ?? string.Empty;
            case QuestionType.MultipleChoice:
                return string.Join("|", question.Answers.Select(a => a.Id));
            case QuestionType.FillBlank:
                // Assuming FillBlank questions have a predefined structure for answers
                return "[{\"1\":\"content\"}]";
            case QuestionType.Matching:
                // Assuming a simple matching format for demonstration
                return "1_2|2_1|4_3|3_4";
            case QuestionType.Writing:
                // Example raw text for Writing question type
                return "This is an example of a raw answer for a writing question.";
            default:
                return string.Empty;
        }
    }
}
