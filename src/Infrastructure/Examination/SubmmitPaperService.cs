using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers.Specs;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
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

    private void ShuffleQuestions(PaperForStudentDto paperDot)
    {
        // Shuffle questions passges and answers
        foreach (var q in paperDot.Questions)
        {
            // Reading question
            if (q.QuestionType == QuestionType.Reading)
            {
                Random random = new Random();
                int n = q.QuestionPassages.Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    (q.QuestionPassages[n], q.QuestionPassages[k]) = (q.QuestionPassages[k], q.QuestionPassages[n]);
                }

                // Shuffle answers
                foreach (var qp in q.QuestionPassages)
                {
                    n = qp.Answers.Count;
                    while (n > 1)
                    {
                        n--;
                        int k = random.Next(n + 1);
                        (qp.Answers[n], qp.Answers[k]) = (qp.Answers[k], qp.Answers[n]);
                    }
                }
            }

            // SingleChoice - MultipleChoice
            if (q.QuestionType is QuestionType.SingleChoice or QuestionType.MultipleChoice)
            {
                Random random = new Random();
                int n = q.Answers.Count;
                while (n > 1)
                {
                    n--;
                    int k = random.Next(n + 1);
                    (q.Answers[n], q.Answers[k]) = (q.Answers[k], q.Answers[n]);
                }
            }
        }

        // Shuffle questions
        Random random1 = new Random();
        int n1 = paperDot.Questions.Count;
        while (n1 > 1)
        {
            n1--;
            int k1 = random1.Next(n1 + 1);
            (paperDot.Questions[n1], paperDot.Questions[k1]) = (paperDot.Questions[k1], paperDot.Questions[n1]);
        }
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

    private void FillPaperDetails(PaperForStudentDto paperDot, SubmitPaper? submitPaper1)
    {
        if (submitPaper1 == null)
        {
            return;
        }

        foreach (var pq in paperDot.Questions)
        {
            var spq = submitPaper1.SubmitPaperDetails.FirstOrDefault(x => x.QuestionId == pq.Id);
            if (spq != null)
            {
                // AnswerRaw to AnswerForStudentDto
                AnswerRawToAnswerForStudentDto(spq, pq);
            }

            if (pq.QuestionType == QuestionType.Reading)
            {
                foreach (var qp in pq.QuestionPassages)
                {
                    var spq1 = submitPaper1.SubmitPaperDetails.FirstOrDefault(x => x.QuestionId == qp.Id);
                    if (spq1 != null)
                    {
                        // AnswerRaw to AnswerForStudentDto
                        AnswerRawToAnswerForStudentDto(spq1, qp);
                    }
                }
            }
        }
    }

    private void AnswerRawToAnswerForStudentDto(SubmitPaperDetail spq1, QuestionPassagesForStudentDto qp)
    {
        // Deserialize AnswerRaw to AnswerForStudentDto
        List<string> answerIds = new List<string>();
        if (!string.IsNullOrEmpty(spq1.AnswerRaw))
        {
            // split answerIds
            string[] answerIdStrs = spq1.AnswerRaw.Split('|');
            foreach (string answerIdStr in answerIdStrs)
            {
                answerIds.Add(answerIdStr);
            }

            // AnswerForStudentDto
            foreach (var a in qp.Answers)
            {
                if (answerIds.Contains(a.Id.ToString()))
                {
                    a.IsCorrect = true;
                }
            }
        }
    }

    private void AnswerRawToAnswerForStudentDto(SubmitPaperDetail spq, QuestionForStudentDto pq)
    {
        // SingleChoice
        if (pq.QuestionType == QuestionType.SingleChoice)
        {
            // AnswerRaw to AnswerForStudentDto
            foreach (var a in pq.Answers)
            {
                if (a.Id == Guid.Parse(spq.AnswerRaw))
                {
                    a.IsCorrect = true;
                }
            }
        }

        // MultipleChoice
        if (pq.QuestionType == QuestionType.MultipleChoice)
        {
            // AnswerRaw to AnswerForStudentDto
            List<string> answerIds = new List<string>();
            if (!string.IsNullOrEmpty(spq.AnswerRaw))
            {
                // split answerIds
                string[] answerIdStrs = spq.AnswerRaw.Split('|');
                foreach (string answerIdStr in answerIdStrs)
                {
                    answerIds.Add(answerIdStr);
                }

                // AnswerForStudentDto
                foreach (var a in pq.Answers)
                {
                    if (answerIds.Contains(a.Id.ToString()))
                    {
                        a.IsCorrect = true;
                    }
                }
            }
        }

        // FillBlank
        if (pq.QuestionType == QuestionType.FillBlank)
        {
            // AnswerRaw to AnswerForStudentDto
            List<Dictionary<string, string>> answers = _serializerService.Deserialize<List<Dictionary<string, string>>>(spq.AnswerRaw);
            for (int i = 0; i < pq.Answers.Count; i++)
            {
                pq.Answers[i].Content = answers[i].Values.First();
            }
        }

        // Writing
        if (pq.QuestionType == QuestionType.Writing)
        {
            // AnswerRaw to AnswerForStudentDto
            // Create AnswerForStudentDto
            pq.Answers = new List<AnswerForStudentDto>
            {
                new()
                {
                    Content = spq.AnswerRaw
                }
            };
        }
    }

    public async Task<PaperForStudentDto> StartExamAsync(StartExamRequest request, CancellationToken cancellationToken)
    {
        var spec = new PaperByIdWithAccessesSpec(request.PaperId);
        var paper = await _paperRepository.FirstOrDefaultAsync(spec, cancellationToken);
        if (paper == null)
        {
            throw new NotFoundException($"Paper {request.PaperId} Not Found.");
        }

        var userId = _currentUser.GetUserId();

        var submitPaper1 = await _submitPaperRepository.FirstOrDefaultAsync(new SubmitPaperByPaperId(paper, userId), cancellationToken);

        // Check time to do this exam
        var timeNow = DateTime.Now;
        if (paper.StartTime.HasValue && paper.StartTime > timeNow)
        {
            throw new ConflictException(_t["Exam has not started yet."]);
        }

        if (paper.EndTime.HasValue && paper.EndTime < timeNow)
        {
            throw new ConflictException(_t["Exam has ended."]);
        }

        // If resume, check user has permission to resume exam
        if (request.IsResume)
        {
            // Get last submit paper
            if (submitPaper1 == null)
            {
                throw new NotFoundException(_t["You do not have permission to resume this exam."]);
            }

            // check can resume
            if (submitPaper1.Status != SubmitPaperStatus.Doing)
            {
                throw new ConflictException(_t["Exam is not in progress."]);
            }

            if (submitPaper1.canResume == false)
            {
                throw new ConflictException(_t["You do not have permission to resume this exam."]);
            }
        }
        else
        {
            // check user has not submitted this paper
            var submitPapers = await _submitPaperRepository.ListAsync(new SubmitPaperByPaperId(paper, userId), cancellationToken);
            if (submitPapers.Count >= paper.NumberAttempt)
            {
                throw new ConflictException(_t["Have used up all your attempts"]);
            }
        }

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

        // check local ip
        if (!string.IsNullOrEmpty(paper.LocalIpAllowed) && !string.IsNullOrEmpty(request.LocalIp) && !IsLocalIpAllowed(request.LocalIp, paper.LocalIpAllowed))
        {
            throw new ForbiddenException(_t["Your local IP: {0} is not allowed to start this exam.", request.LocalIp]);
        }

        // check public ip
        // if (!string.IsNullOrEmpty(paper.PublicIpAllowed) && !string.IsNullOrEmpty(request.PublicIp) && !IsIpInRange(request.PublicIp, paper.PublicIpAllowed))
        // {
        //    throw new ForbiddenException("Your public IP: " + request.PublicIp + " is not allowed to start this exam.");
        // }

        if (request.IsResume)
        {
            var paperDot = paper.Adapt<PaperForStudentDto>();

            // Refill submit paper details
            FillPaperDetails(paperDot, submitPaper1);

            // Set Resume to false
            submitPaper1.canResume = false;
            submitPaper1.StartTime = timeNow;
            await _submitPaperRepository.UpdateAsync(submitPaper1, cancellationToken);

            var user = await _userService.GetAsync(submitPaper1.CreatedBy.ToString(), cancellationToken);
            paperDot.SubmitPaperId = submitPaper1.Id;
            paperDot.UserDetails = user.Adapt<UserDetailsDto>();

            // Shuffle questions
            if (paper.Shuffle)
            {
                ShuffleQuestions(paperDot);
            }

            return paperDot;
        }
        else
        {
            // Create new submit paper
            var submitPaper = new SubmitPaper
            {
                PaperId = paper.Id,
                Status = SubmitPaperStatus.Doing,
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

            // Shuffle questions
            if (paper.Shuffle)
            {
                ShuffleQuestions(paperDot);
            }

            return paperDot;
        }

    }

    public async Task<DefaultIdType> SubmitExamAsync(SubmitExamRequest request, CancellationToken cancellationToken)
    {
        // Decrypt and validate submit paper data
        string submitPaperDataDecrypted = EncryptionUtils.SimpleDec(request.SubmitPaperData);
        if (string.IsNullOrEmpty(submitPaperDataDecrypted))
        {
            throw new BadRequestException(_t["Submit paper data is invalid."]);
        }

        // Json to object SubmitPaperData
        var sbp = _serializerService.Deserialize<SubmitPaperData>(submitPaperDataDecrypted);

        // Get submit paper
        var submitPaper = await _submitPaperRepository.FirstOrDefaultAsync(new SubmitPaperByIdSpec(Guid.Parse(sbp.SubmitPaperId!)), cancellationToken);

        // Check current user has permission to submit this exam
        if (submitPaper == null || submitPaper.CreatedBy != _currentUser.GetUserId())
        {
            throw new ForbiddenException(_t["You do not have permission to submit this exam."]);
        }

        // Check submit paper status
        if (submitPaper.Status != SubmitPaperStatus.Doing)
        {
            throw new ConflictException(_t["Exam is not in progress."]);
        }

        // Check time to do this exam
        var paper = await _paperRepository.FirstOrDefaultAsync(new PaperByIdSpec(submitPaper.PaperId), cancellationToken);
        var timeNow = DateTime.Now;
        int duration = (int)(timeNow - submitPaper.StartTime).TotalMinutes;
        if (paper.Duration.HasValue && duration > paper.Duration)
        {
            // Update submit paper status
            submitPaper.Status = SubmitPaperStatus.End;
            submitPaper.EndTime = timeNow;
            await _submitPaperRepository.UpdateAsync(submitPaper, cancellationToken);
            throw new ConflictException(_t["Over time to do this exam."]);
        }

        // Update or Add submit paper details
        foreach (var q in sbp.Questions)
        {
            // check question is exist in paper
            var question = submitPaper.SubmitPaperDetails?.FirstOrDefault(x => x.QuestionId == Guid.Parse(q.Id!));
            var questionDb = submitPaper.Paper?.PaperQuestions?.FirstOrDefault(x => x.QuestionId == Guid.Parse(q.Id!)).Question;
            if (question == null)
            {
                // Add new submit paper detail
                if (questionDb != null)
                {

                    // Add new submit paper detail
                    SubmitPaperDetail submitPaperDetail;
                    submitPaperDetail = new SubmitPaperDetail
                    {
                        QuestionId = Guid.Parse(q.Id!),
                        AnswerRaw = FormatAnswerRaw(q, questionDb),
                        SubmitPaperId = submitPaper.Id
                    };

                    submitPaper.SubmitPaperDetails.Add(submitPaperDetail);

                    if (questionDb.QuestionType == QuestionType.Reading)
                    {
                        foreach (var pq in questionDb.QuestionPassages)
                        {
                            foreach (var q1 in q.QuestionPassages)
                            {
                                if (pq.Id == Guid.Parse(q1.Id!))
                                {
                                    SubmitPaperDetail spdp;
                                    spdp = new SubmitPaperDetail
                                    {
                                        QuestionId = Guid.Parse(q1.Id!),
                                        AnswerRaw = FormatAnswerRaw(q1, pq),
                                        SubmitPaperId = submitPaper.Id
                                    };

                                    submitPaper.SubmitPaperDetails.Add(spdp);
                                }
                            }
                        }
                    }

                }

            }
            else
            {
                question.AnswerRaw = FormatAnswerRaw(q, question.Question!);
                if (questionDb.QuestionType == QuestionType.Reading)
                {
                    foreach (var pq in questionDb.QuestionPassages)
                    {
                        foreach (var q1 in q.QuestionPassages)
                        {
                            if (pq.Id == Guid.Parse(q1.Id!))
                            {
                                var spdp = submitPaper.SubmitPaperDetails!.FirstOrDefault(x => x.QuestionId == Guid.Parse(q1.Id!));
                                if (spdp != null)
                                {
                                    spdp.AnswerRaw = FormatAnswerRaw(q1, pq);
                                }
                            }
                        }
                    }
                }
            }
        }

        // if isEnd
        if (request.IsEnd)
        {
            // Update submit paper status
            submitPaper.Status = SubmitPaperStatus.End;
            submitPaper.EndTime = timeNow;
        }

        // Update submit paper
        await _submitPaperRepository.UpdateAsync(submitPaper!, cancellationToken);

        return submitPaper == null
            ? throw new NotFoundException(_t["Submit paper {0} not found.", sbp.SubmitPaperId])
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

}
