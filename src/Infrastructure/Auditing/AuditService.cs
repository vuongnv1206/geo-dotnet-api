using DocumentFormat.OpenXml.Vml.Office;
using FSH.WebApi.Application.Auditing;
using FSH.WebApi.Application.Auditing.Class;
using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Models;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Infrastructure.Common.Utils;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace FSH.WebApi.Infrastructure.Auditing;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context) => _context = context;

    public async Task<List<AuditDto>> GetUserTrailsAsync(Guid userId)
    {
        var trails = await _context.AuditTrails
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.DateTime)
            .Take(250)
            .ToListAsync();

        return trails.Adapt<List<AuditDto>>();
    }

    public async Task<PaginationResponse<AuditTrailsDto>> GetClassTrailsAsync(GetClassLogsRequest request)
    {
        var query = _context.AuditTrails
             .Where(a => a.TableName == "Classes" && a.UserId.Equals(request.UserId))
             .Where(a => !string.IsNullOrEmpty(request.Action) ? a.Type.ToLower() == request.Action.ToLower() : true)
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
            .Select(a => new AuditTrailsDto
            {
                Id = a.AuditTrail.Id.ToString(),
                Author = new AuthorDto
                {
                    Id = a.User.Id,
                    Name = a.User.FirstName + " " + a.User.LastName,
                    Email = a.User.Email,
                    ImageUrl = a.User.ImageUrl
                },
                Action = a.AuditTrail.Type,
                Resource = a.AuditTrail.TableName,
                ResourceId = JsonUtils.GetJsonProperties(a.AuditTrail.PrimaryKey, "id"),
                CreatedAt = a.AuditTrail.DateTime
            })
            .ToListAsync();

        return new PaginationResponse<AuditTrailsDto>(trails, totalRecords, request.PageNumber, request.PageSize);
    }

    public async Task<ClassLogDto> GetClassLogCreateDetails(Guid id)
    {
        var auditTrail = await _context.AuditTrails
            .FirstOrDefaultAsync(a => a.Id == id);

        if (auditTrail == null) throw new NotFoundException("Audit trail not found");

        var classDTO = new ClassLogDto
        {
            Id = Guid.Parse(JsonUtils.GetJsonProperties(auditTrail.PrimaryKey, "id")),
            Name = JsonUtils.GetJsonProperties(auditTrail.NewValues, "Name"),
            SchoolYear = JsonUtils.GetJsonProperties(auditTrail.NewValues, "SchoolYear"),
            GroupClassId = Guid.Parse(JsonUtils.GetJsonProperties(auditTrail.NewValues, "GroupClassId")),
            GroupName = await _context.GroupClasses.IgnoreQueryFilters()
                .Where(g => g.Id == Guid.Parse(JsonUtils.GetJsonProperties(auditTrail.NewValues, "GroupClassId")))
                .Select(g => g.Name)
                .FirstOrDefaultAsync()
        };

        return classDTO;
    }

    public async Task<AuditTrailsUpdateDetailsDto> GetClassUpdateLogDetails(Guid id)
    {
        var auditTrail = await _context.AuditTrails
            .Where(a => a.Type.Equals("Update"))
            .FirstOrDefaultAsync(a => a.Id == id);
        if (auditTrail == null)
        {
            throw new NotFoundException("Audit trail not found");
        }

        string[] changeFields = JsonUtils.SplitStringArray(auditTrail.AffectedColumns);
        string classId = JsonUtils.GetJsonProperties(auditTrail.PrimaryKey, "id");
        Dictionary<string, string> oldData = new();
        Dictionary<string, string> newData = new();

        foreach (var field in changeFields)
        {
            oldData.Add(field, JsonUtils.GetJsonProperties(auditTrail.OldValues, field));
            newData.Add(field, JsonUtils.GetJsonProperties(auditTrail.NewValues, field));
        }

        if (changeFields.Contains("GroupClassId"))
        {
            oldData.Add("GroupName", await _context.GroupClasses.IgnoreQueryFilters()
                .Where(g => g.Id == Guid.Parse(oldData["GroupClassId"]))
                .Select(g => g.Name)
                .FirstOrDefaultAsync());
            newData.Add("GroupName", await _context.GroupClasses.IgnoreQueryFilters()
                .Where(g => g.Id == Guid.Parse(newData["GroupClassId"]))
                .Select(g => g.Name)
                .FirstOrDefaultAsync());
        }

        return new()
        {
            Id = classId,
            OldData = oldData,
            NewData = newData,
            ChangeFields = changeFields
        };
    }

    public async Task<ClassLogDto> GetClassLogDeleteDetails(Guid classId)
    {
        ClassLogDto classLogDto = await _context.Classes.Include(c => c.GroupClass)
            .IgnoreQueryFilters()
            .Select(c => new ClassLogDto()
            {
                Id = c.Id,
                Name = c.Name,
                SchoolYear = c.SchoolYear,
                GroupClassId = c.GroupClassId,
                GroupName = c.GroupClass.Name
            })
            .FirstOrDefaultAsync(c => c.Id == classId);

        if (classLogDto == null)
        {
            throw new NotFoundException("Class not found");
        }

        return classLogDto;
    }

}