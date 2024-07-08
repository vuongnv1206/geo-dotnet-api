using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Host.Controllers.Examination;
using FSH.WebApi.Infrastructure.Persistence.Context;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSH.WebApi.Infrastructure.Examination;
public class SubmmitPaperService : ISubmmitPaperService
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<Paper> _paperRepository;
    private readonly IRepository<SubmitPaper> _submitPaperRepository;
    private readonly ICurrentUser _currentUser;

    public SubmmitPaperService(ApplicationDbContext context, IRepository<Paper> paperRepository, IRepository<SubmitPaper> submitPaperRepository, ICurrentUser currentUser)
    {
        _context = context;
        _paperRepository = paperRepository;
        _submitPaperRepository = submitPaperRepository;
        _currentUser = currentUser;
    }

    private bool IsIpInRange(string ip, string ipRange)
    {
        try
        {
            // Split the IP range into IP address and CIDR subnet mask
            string[] parts = ipRange.Split('/');
            if (parts.Length != 2) return false;

            var ipAddress = IPAddress.Parse(parts[0]);
            int prefixLength = int.Parse(parts[1]);

            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] ipToCheckBytes = IPAddress.Parse(ip).GetAddressBytes();

            if (ipBytes.Length != ipToCheckBytes.Length) return false;

            // Create a mask with the specified prefix length
            byte[] mask = new byte[ipBytes.Length];
            for (int i = 0; i < mask.Length; i++)
            {
                if (prefixLength >= 8)
                {
                    mask[i] = 255;
                    prefixLength -= 8;
                }
                else
                {
                    mask[i] = (byte)(256 - (1 << (8 - prefixLength)));
                    prefixLength = 0;
                }
            }

            // Apply the mask and compare
            for (int i = 0; i < ipBytes.Length; i++)
            {
                if ((ipBytes[i] & mask[i]) != (ipToCheckBytes[i] & mask[i]))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    private bool IsLocalIpAllowed(string localIp, string localIpAllowed)
    {
        // Split the allowed IP ranges
        string[] ranges = localIpAllowed.Split(';');
        foreach (string range in ranges)
        {
            if (IsIpInRange(localIp, range))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<PaperForStudentDto> StartExamAsync(StartExamRequest request, CancellationToken cancellationToken)
    {
        // check current user is allowed to start exam
        // check paper is available
        // check user has not submitted this paper
        // create new submit paper
        // return paper for student dto

        var spec = new PaperByIdWithAccessesSpec(request.PaperId);
        var paper = await _paperRepository.FirstOrDefaultAsync(spec, cancellationToken);
        if (paper == null)
        {
            throw new NotFoundException($"Paper {request.PaperId} Not Found.");
        }

        var userId = _currentUser.GetUserId();

        //// check user has permission to start exam
        //bool hasPermission = false;
        //if (!paper.PaperAccesses.Any(x => x.UserId == userId))
        //{
        //    hasPermission = true;
        //}

        //if (!paper.PaperAccesses.Any(x => x.Class.UserClasses.Any(y => y.StudentId == userId)))
        //{
        //    hasPermission = true;
        //}

        //if (!hasPermission)
        //{
        //    throw new ForbiddenException("You do not have permission to start this exam.");
        //}

        // check user has not submitted this paper
        var submitPapers = await _submitPaperRepository.ListAsync(new SubmitPaperByPaperId(paper, userId), cancellationToken);
        if (submitPapers.Count >= paper.NumberAttempt)
        {
            throw new ConflictException("Have used up all your attempts");
        }

        // check local ip
        if (!string.IsNullOrEmpty(paper.LocalIpAllowed) && !string.IsNullOrEmpty(request.LocalIp) && !IsLocalIpAllowed(request.LocalIp, paper.LocalIpAllowed))
        {
            throw new ForbiddenException("Your local IP: " + request.LocalIp + " is not allowed to start this exam.");
        }

        // check public ip
        //if (!string.IsNullOrEmpty(paper.PublicIpAllowed) && !string.IsNullOrEmpty(request.PublicIp) && !IsIpInRange(request.PublicIp, paper.PublicIpAllowed))
        //{
        //    throw new ForbiddenException("Your public IP: " + request.PublicIp + " is not allowed to start this exam.");
        //}

        var submitPaper = new SubmitPaper
        {
            PaperId = paper.Id,
            DeviceId = request.DeviceId,
            DeviceName = request.DeviceName,
            DeviceType = request.DeviceType,
            PublicIp = request.PublicIp,
            LocalIp = request.LocalIp
        };

        await _submitPaperRepository.AddAsync(submitPaper, cancellationToken);

        var paperDot = paper.Adapt<PaperForStudentDto>();

        return paperDot;
    }
}
