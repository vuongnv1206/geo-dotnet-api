using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Payment;
using FSH.WebApi.Infrastructure.Notifications;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Infrastructure.Persistence.Initialization;
using FSH.WebApi.Shared.Authorization;
using Microsoft.Extensions.Logging;

namespace FSH.WebApi.Infrastructure.Payment;
public class SubscriptionSeeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SubscriptionSeeder> _logger;

    public SubscriptionSeeder(ISerializerService serializerService, ApplicationDbContext db, ILogger<SubscriptionSeeder> logger)
    {
        _serializerService = serializerService;
        _db = db;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (!_db.Subscriptions.Any())
        {
            _logger.LogInformation("Started to Seed Subscriptions.");
            var subscriptions = new List<Subscription>
            {
                new Subscription { Name = FSHRoles.Basic, Role = FSHRoles.Basic, Image = "<i class=\"fa-regular fa-user\"></i>", Price = 1000, Description = "The Basic Subscription offers essential features to get started with our education system. This tier is perfect for individual learners or small groups who need access to core learning materials and tools."},
                new Subscription { Name = FSHRoles.Standard, Role = FSHRoles.Standard, Image = "<i class=\"fa-regular fa-user\"></i>", Price = 5000, Description = "The Standard Subscription provides an enhanced learning experience with additional resources and support. This tier is ideal for students and educators who require more comprehensive tools and materials to facilitate effective learning and teaching." },
                new Subscription { Name = FSHRoles.Professional, Role = FSHRoles.Professional, Image = "<i class=\"fa-regular fa-user\"></i>", Price = 10000, Description = "The Professional Subscription offers the most extensive and immersive educational experience. This tier is designed for institutions, professional educators, and serious learners who need a full suite of tools and resources to achieve their educational goals." }
            };

            _db.Subscriptions.AddRange(subscriptions);
            _ = await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded Subscriptions.");
        }
    }

}
