using FSH.WebApi.Application.Auditing;
using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Infrastructure.Common.Utils;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FSH.WebApi.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context) => _context = context;

    public Task<List<string>> GetResourceName()
    {
        return _context.AuditTrails
            .Select(a => a.TableName)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<PaginationResponse<AuditDto>> GetUserTrailsAsync(GetMyAuditLogsRequest request)
    {
        var query = _context.AuditTrails
            .Where(a => a.UserId.Equals(request.UserId))
            .Where(a => !string.IsNullOrEmpty(request.Action) ? a.Type.ToLower() == request.Action.ToLower() : true)
            .Where(a => !string.IsNullOrEmpty(request.Resource) ? a.TableName.ToLower() == request.Resource.ToLower() : true)
            .Join(
                _context.Users,
                a => a.UserId.ToString(),
                u => u.Id,
                (a, u) => new { AuditTrail = a, User = u })
            .OrderByDescending(a => a.AuditTrail.DateTime)
            .AsQueryable();

        var totalRecords = await query.CountAsync();

        var trails = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AuditDto
            {
                Id = a.AuditTrail.Id,
                Author = new AuthorDto
                {
                    Id = a.User.Id,
                    Name = a.User.FirstName + " " + a.User.LastName,
                    Email = a.User.Email,
                    ImageUrl = a.User.ImageUrl
                },
                Action = a.AuditTrail.Type,
                Resource = a.AuditTrail.TableName,
                OldValues = a.AuditTrail.OldValues,
                NewValues = a.AuditTrail.NewValues,
                AffectedColumns = JsonUtils.SplitStringArray(a.AuditTrail.AffectedColumns),
                ResourceId = JsonUtils.GetJsonProperties(a.AuditTrail.PrimaryKey, "id"),
                CreatedAt = a.AuditTrail.DateTime
            })
            .ToListAsync();

        return new PaginationResponse<AuditDto>(trails, totalRecords, request.PageNumber, request.PageSize);
    }
}