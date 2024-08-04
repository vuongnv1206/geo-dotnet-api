using FSH.WebApi.Application.Notifications;
using FSH.WebApi.Application.Payment;
using FSH.WebApi.Domain.Payment;
using FSH.WebApi.Infrastructure.Identity;
using FSH.WebApi.Infrastructure.Persistence.Context;
using FSH.WebApi.Shared.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace FSH.WebApi.Infrastructure.Payment;
public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IOptions<PaymentSettings> _settings;

    public PaymentService(ILogger<PaymentService> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService, IOptions<PaymentSettings> settings)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _notificationService = notificationService;
        _settings = settings;
    }

    public async Task CheckNewTransactions()
    {
        _logger.LogInformation("Syncing new transactions...");
        List<Transaction> newTransactions = await GetNewTransaction();
        if (newTransactions.Count == 0)
        {
            _logger.LogInformation("No new transactions.");
            return;
        }

        foreach (var transaction in newTransactions)
        {
            Order order = _context.Orders.Include(o => o.Subscription).FirstOrDefault(o => o.OrderNo == transaction.Description);
            if (order == null)
            {
                transaction.IsSuccess = false;
                transaction.ErrorMessage = "Order not found.";
            }

            if (order != null && order.Subscription.Price > transaction.Amount)
            {
                transaction.IsSuccess = false;
                transaction.ErrorMessage = "Amount not enough.";
            }

            if (transaction.IsSuccess)
            {
                // Update order
                order.Status = OrderStatus.COMPLETED;
                order.StartDate = DateOnly.FromDateTime(DateTime.Now);
                order.EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(order.Subscription.Duration));
                _context.Update(order);

                // Add role to user
                var user = await _userManager.FindByIdAsync(order.UserId.ToString());
                await _userManager.AddToRoleAsync(user, order.Subscription.Role);

                // Send notification
                var notification = new BasicNotification
                {
                    Message = "Your order payment has been completed.",
                    Label = BasicNotification.LabelType.Information,
                    Title = "Subscription Payment",
                    Url = "/orders"
                };

                await _notificationService.SendNotificationToUser(order.UserId.ToString(), notification, null, default);
            }

            await _context.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Added {count} new transactions.", newTransactions.Count);
    }

    private async Task<List<Transaction>> SyncTransactions()
    {
        HttpClient client = new HttpClient();
        var response = await client.GetAsync(_settings.Value.TransactionsURL);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<TransactionAPIResponse>();
            if (content.Status && content.Transactions.Count > 0)
            {

                return content.Transactions.Select(t => new Transaction
                {
                    TransactionID = t.TransactionID,
                    Amount = t.Amount,
                    Description = t.Description,
                    TransactionDate = DateOnly.ParseExact(t.TransactionDate, "dd/MM/yyyy"),
                    Type = t.Type == "IN" ? TransactionType.IN : TransactionType.OUT,
                    IsSuccess = true
                }).Where(t => t.Type.Equals(TransactionType.IN)).ToList();
            }
        }
        else
        {
            _logger.LogError("Failed to get new transactions from banking.");
        }

        return new List<Transaction>();
    }

    private async Task<List<Transaction>> GetNewTransaction()
    {
        List<Transaction> syncTransactions = await SyncTransactions();
        if (syncTransactions.Count == 0) return syncTransactions;
        //List<Transaction> todayTransaction = await _context.Transactions.Where(t => t.TransactionDate.Equals(DateOnly.FromDateTime(DateTime.Now))).ToListAsync();
        List<Transaction> todayTransaction = await _context.Transactions.ToListAsync();
        return syncTransactions.Where(t => !todayTransaction.Any(tt => tt.TransactionID == t.TransactionID)).ToList();
    }

    public async Task DeactiveExpiredUser()
    {
        foreach (var order in await _context.Orders.Include(o => o.Subscription).Where(o => o.EndDate < DateOnly.FromDateTime(DateTime.Now) && !o.IsExpired).ToListAsync())
        {
            var user = _userManager.FindByIdAsync(order.UserId.ToString()).Result;
            await _userManager.RemoveFromRoleAsync(user, order.Subscription.Role);
            order.IsExpired = true;
            _context.Update(order);
            await _context.SaveChangesAsync();

            var notification = new BasicNotification
            {
                Message = "Your subscription has been expired.",
                Label = BasicNotification.LabelType.Warning,
                Title = "Subscription Expired",
                Url = "/orders"
            };

            await _notificationService.SendNotificationToUser(order.UserId.ToString(), notification, null, default);
        }
    }

    public async Task<List<SubcriptionDto>> GetSubcriptions()
    {
        return await _context.Subscriptions.Select(s => new SubcriptionDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Price = s.Price,
            Duration = s.Duration,
            Image = s.Image
        }).ToListAsync();
    }
}
