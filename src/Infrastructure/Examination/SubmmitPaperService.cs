using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using FSH.WebApi.Host.Controllers.Examination;
using FSH.WebApi.Infrastructure.Common.Utils;
using Mapster;
using Microsoft.Extensions.Localization;
using System.Net;

namespace FSH.WebApi.Infrastructure.Examination;
public class SubmmitPaperService : ISubmmitPaperService
{
    private readonly IRepository<Paper> _paperRepository;
    private readonly IRepository<SubmitPaper> _submitPaperRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<SubmmitPaperService> _t;
    private readonly ISerializerService _serializerService;

    public SubmmitPaperService(IRepository<Paper> paperRepository, IRepository<SubmitPaper> submitPaperRepository, ICurrentUser currentUser, IUserService userService, IStringLocalizer<SubmmitPaperService> t, ISerializerService serializerService)
    {
        _paperRepository = paperRepository;
        _submitPaperRepository = submitPaperRepository;
        _currentUser = currentUser;
        _userService = userService;
        _t = t;
        _serializerService = serializerService;
    }

    private bool IsIpInRange(string ip, string ipRange)
    {
        try
        {
            // Split the IP range into IP address and CIDR subnet mask
            string[] parts = ipRange.Split('/');
            if (parts.Length != 2)
            {
                return false;
            }

            var ipAddress = IPAddress.Parse(parts[0]);
            int prefixLength = int.Parse(parts[1]);

            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] ipToCheckBytes = IPAddress.Parse(ip).GetAddressBytes();

            if (ipBytes.Length != ipToCheckBytes.Length)
            {
                return false;
            }

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
        catch (Exception)
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
        // bool hasPermission = false;
        // if (!paper.PaperAccesses.Any(x => x.UserId == userId))
        // {
        //    hasPermission = true;
        // }

        // if (!paper.PaperAccesses.Any(x => x.Class.UserClasses.Any(y => y.StudentId == userId)))
        // {
        //    hasPermission = true;
        // }

        // if (!hasPermission)
        // {
        //    throw new ForbiddenException("You do not have permission to start this exam.");
        // }

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
        // if (!string.IsNullOrEmpty(paper.PublicIpAllowed) && !string.IsNullOrEmpty(request.PublicIp) && !IsIpInRange(request.PublicIp, paper.PublicIpAllowed))
        // {
        //    throw new ForbiddenException("Your public IP: " + request.PublicIp + " is not allowed to start this exam.");
        // }

        var submitPaper = new SubmitPaper
        {
            PaperId = paper.Id,
            DeviceId = request.DeviceId,
            DeviceName = request.DeviceName,
            DeviceType = request.DeviceType,
            PublicIp = request.PublicIp,
            LocalIp = request.LocalIp
        };

        _ = await _submitPaperRepository.AddAsync(submitPaper, cancellationToken);

        var paperDot = paper.Adapt<PaperForStudentDto>();
        var user = await _userService.GetAsync(submitPaper.CreatedBy.ToString(), cancellationToken);
        paperDot.SubmitPaperId = submitPaper.Id;
        paperDot.UserDetails = user.Adapt<UserDetailsDto>();
        return paperDot;
    }

    public async Task<DefaultIdType> SubmitExamAsync(SubmitExamRequest request, CancellationToken cancellationToken)
    {
        // Decrypt and validate submit paper data
        string submitPaperDataDecrypted = EncryptionUtils.SimpleDec(request.SubmitPaperData);
        if (string.IsNullOrEmpty(submitPaperDataDecrypted))
        {
            throw new BadRequestException("Submit paper data is invalid.");
        }

        // Json to object SubmitPaperData
        var sbp = _serializerService.Deserialize<SubmitPaperData>(submitPaperDataDecrypted);

        // Get submit paper
        var submitPaper = await _submitPaperRepository.FirstOrDefaultAsync(new SubmitPaperByIdSpec(Guid.Parse(sbp.SubmitPaperId!)), cancellationToken);

        // Update or Add submit paper details
        foreach (var q in sbp.Questions)
        {
            // check question is exist in paper
            var question = submitPaper.SubmitPaperDetails?.FirstOrDefault(x => x.QuestionId == Guid.Parse(q.Id!));
            if (question == null)
            {
                var questionDb = submitPaper.Paper?.PaperQuestions?.FirstOrDefault(x => x.QuestionId == Guid.Parse(q.Id!)).Question;

                // Add new submit paper detail
                SubmitPaperDetail submitPaperDetail;
                if (questionDb != null)
                {
                    submitPaperDetail = new SubmitPaperDetail
                    {
                        QuestionId = Guid.Parse(q.Id!),
                        AnswerRaw = FormatAnswerRaw(q, questionDb),
                        SubmitPaperId = submitPaper.Id
                    };

                    submitPaper.SubmitPaperDetails.Add(submitPaperDetail);
                }
            }
            else
            {
                // Update submit paper detail
                question.AnswerRaw = FormatAnswerRaw(q, question.Question!);
            }
        }

        // Update submit paper
        await _submitPaperRepository.UpdateAsync(submitPaper!, cancellationToken);

        return submitPaper == null
            ? throw new NotFoundException("Submit Paper " + sbp.SubmitPaperId + " Not Found.")
            : await Task.FromResult(submitPaper.Id);
    }

    public string FormatAnswerRaw(SubmitPaperQuestion spq, QuestionClone question)
    {
        if (question == null)
        {
            return string.Empty;
        }

        // SingleChoice
        if (question.QuestionType == QuestionType.SingleChoice)
        {
            return FormatSingleChoiceAnswerRaw(spq, question);
        }

        // MultipleChoice - Reading - Question Passage
        if (question.QuestionType is QuestionType.MultipleChoice or QuestionType.ReadingQuestionPassage)
        {
            return FormatMultipleChoiceAnswerRaw(spq, question);
        }

        // FillBlank
        if (question.QuestionType == QuestionType.FillBlank)
        {
            return FormatFillBlankAnswerRaw(spq, question);
        }

        // Matching
        if (question.QuestionType == QuestionType.Matching)
        {
            return spq.Answers!.FirstOrDefault()?.Content ?? string.Empty;
        }

        // Writing
        return question.QuestionType == QuestionType.Writing ? spq.Answers!.FirstOrDefault()?.Content ?? string.Empty : string.Empty;
    }

    private string FormatSingleChoiceAnswerRaw(SubmitPaperQuestion spq, QuestionClone question)
    {
        // SingleChoice : answerId
        foreach (var a in question.AnswerClones)
        {
            foreach (var s in spq.Answers)
            {
                if (a.Id == Guid.Parse(s.Id!) && s.IsCorrect)
                {
                    return a.Id.ToString();
                }
            }
        }

        return string.Empty;
    }

    private string FormatMultipleChoiceAnswerRaw(SubmitPaperQuestion spq, QuestionClone question)
    {
        // MultipleChoise : answerId|answerId
        var answerIds = new List<string>();
        foreach (var a in question.AnswerClones)
        {
            foreach (var s in spq.Answers)
            {
                if (a.Id == Guid.Parse(s.Id!) && s.IsCorrect)
                {
                    answerIds.Add(a.Id.ToString());
                }
            }
        }

        return string.Join('|', answerIds);
    }

    private string FormatFillBlankAnswerRaw(SubmitPaperQuestion spq, QuestionClone question)
    {
        List<Dictionary<string, string>> answers = new();
        foreach (var a in question.AnswerClones)
        {
            int index = 1;
            foreach (var s in spq.Answers)
            {
                if (a.Id == Guid.Parse(s.Id!))
                {
                    if (!string.IsNullOrEmpty(s.Content))
                    {
                        answers.Add(new Dictionary<string, string> { { index.ToString(), s.Content! } });
                    }
                }

                index++;
            }
        }

        // Serialize to json
        return _serializerService.Serialize(answers);
    }

}
