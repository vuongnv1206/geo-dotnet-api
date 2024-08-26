using FSH.WebApi.Application.Common.Exceptions;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Application.Examination.Monitor;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.Reviews;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers.Specs;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Examination.Enums;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using FSH.WebApi.Infrastructure.Common.Utils;
using Mapster;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Text.RegularExpressions;

namespace FSH.WebApi.Infrastructure.Examination;
public class SubmmitPaperService : ISubmmitPaperService
{
    private readonly IRepository<Paper> _paperRepository;
    private readonly IRepository<SubmitPaper> _submitPaperRepository;
    private readonly IRepository<SubmitPaperLog> _submitPaperLogRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUserService _userService;
    private readonly IStringLocalizer<SubmmitPaperService> _t;
    private readonly ISerializerService _serializerService;

    public SubmmitPaperService(IRepository<Paper> paperRepository, IRepository<SubmitPaper> submitPaperRepository, IRepository<SubmitPaperLog> submitPaperLogRepository, ICurrentUser currentUser, IUserService userService, IStringLocalizer<SubmmitPaperService> t, ISerializerService serializerService)
    {
        _paperRepository = paperRepository;
        _submitPaperRepository = submitPaperRepository;
        _submitPaperLogRepository = submitPaperLogRepository;
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
                // check if not null
                if (string.IsNullOrEmpty(spq.AnswerRaw!))
                {
                    continue;
                }

                if (a.Id == Guid.Parse(spq.AnswerRaw!))
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
                    if (answerIds.Contains(a.Id.ToString()!))
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
            for (int i = 0; i < answers.Count; i++)
            {
                pq.Answers[i].Content = answers[i].Values.First();
            }
        }

        // Matching
        if (pq.QuestionType == QuestionType.Matching)
        {
            if (pq.Answers.Count > 0)
            {
                pq.Answers[0].Content = spq.AnswerRaw;
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

    public async Task<string> SubmitExamAsync(SubmitExamRequest request, CancellationToken cancellationToken)
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

            // Update submit paper
            await _submitPaperRepository.UpdateAsync(submitPaper, cancellationToken);

            // Calculate score
            submitPaper.TotalMark = await CalculateScoreSubmitPaper(submitPaper);

            // Update submit paper
            await _submitPaperRepository.UpdateAsync(submitPaper, cancellationToken);
        }
        else
        {
            await _submitPaperRepository.UpdateAsync(submitPaper!, cancellationToken);
        }

        if (paper.ShowMarkResult == ShowResult.No)
        {
            return _t["Submit exam successfully."];
        }
        else
        {
            float? mark = submitPaper.TotalMark / paper.Adapt<PaperDto>().MaxPoint * 10;
            return mark.HasValue ? mark.Value.ToString() : "0";
        }
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

    public async Task<float> CalculateScoreSubmitPaper(DefaultIdType submitPaperId)
    {
        var submitPaper = await _submitPaperRepository.FirstOrDefaultAsync(new SubmitPaperByIdSpec(submitPaperId));
        return submitPaper == null
            ? throw new NotFoundException(_t["Submit paper {0} not found.", submitPaperId])
            : await CalculateScoreSubmitPaper(submitPaper);
    }

    private async Task<float> CalculateScoreSubmitPaper(SubmitPaper submitPaper)
    {
        var paper = await _paperRepository.FirstOrDefaultAsync(new PaperByIdSpec(submitPaper.PaperId));
        if (paper == null)
        {
            throw new NotFoundException(_t["Paper {0} not found.", submitPaper.PaperId]);
        }

        foreach (var question in paper.PaperQuestions)
        {
            var detail = submitPaper.SubmitPaperDetails.FirstOrDefault(x => x.QuestionId == question.QuestionId);
            if (detail != null)
            {
                // if question is Reading
                if (question.Question!.QuestionType == QuestionType.Reading)
                {
                    // list of question passages detail
                    List<SubmitPaperDetail> details = new();
                    foreach (var qp in question.Question!.QuestionPassages)
                    {
                        var detail1 = submitPaper.SubmitPaperDetails.FirstOrDefault(x => x.QuestionId == qp.Id);
                        if (detail1 != null)
                        {
                            details.Add(detail1);
                        }
                    }

                    detail.Mark = CalculateScore(detail, question.Question!, question.Mark, details);
                }
                else
                {
                    detail.Mark = CalculateScore(detail, question.Question!, question.Mark);
                }

            }
        }

        float totalMark = 0;
        foreach (var item in submitPaper.SubmitPaperDetails)
        {
            if (item.Mark.HasValue)
            {
                totalMark += item.Mark.Value;
            }
        }

        return totalMark;
    }

    private float CalculateScore(SubmitPaperDetail detail, QuestionClone question, float mark, List<SubmitPaperDetail>? details = null)
    {
        return question.QuestionType switch
        {
            QuestionType.SingleChoice or QuestionType.ReadingQuestionPassage => CalculateSingleChoiceScore(detail, question, mark),
            QuestionType.MultipleChoice => CalculateMultipleChoiceScore(detail, question, mark),
            QuestionType.FillBlank => CalculateFillBlankScore(detail, question, mark),
            QuestionType.Matching => CalculateMatchingScore(detail, question, mark),
            QuestionType.Writing => CalculateWritingScore(detail),
            QuestionType.Reading => CalculateReadingScore(question, mark, details),
            _ => 0,
        };
    }

    private float CalculateMultipleChoiceScore(SubmitPaperDetail detail, QuestionClone question, float mark)
    {
        // MultipleChoice
        // Get answerIds from AnswerRaw
        string[] answerIdStrs = detail.AnswerRaw!.Split('|');
        bool isCorrect = true;
        foreach (var a in question.AnswerClones)
        {
            if (a.IsCorrect && !answerIdStrs.Contains(a.Id.ToString()))
            {
                isCorrect = false;
                break;
            }
        }

        return isCorrect ? mark : 0;
    }

    private float CalculateSingleChoiceScore(SubmitPaperDetail detail, QuestionClone question, float mark)
    {
        // SingleChoice
        foreach (var a in question.AnswerClones)
        {
            try
            {
                if (a.Id == Guid.Parse(detail.AnswerRaw!) && a.IsCorrect)
                {
                    return mark;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        return 0;
    }

    private float CalculateFillBlankScore(SubmitPaperDetail detail, QuestionClone question, float mark)
    {
        // FillBlank
        List<Dictionary<string, string>> answers = _serializerService.Deserialize<List<Dictionary<string, string>>>(detail.AnswerRaw!);

        // question.AnswerClones array = ["$_[1]Sahara", "$_[2]Africa"]
        // Convert to dictionary
        List<Dictionary<string, string>> correctAnswers = new();

        // 1. Sahara 2. Africa
        Regex regex = new Regex(@"\$_\[(\d+)\](.*)");
        foreach (var a in question.AnswerClones)
        {
            Match match = regex.Match(a.Content!);
            if (match.Success)
            {
                correctAnswers.Add(new Dictionary<string, string> { { match.Groups[1].Value, match.Groups[2].Value } });
            }
        }

        int correctCount = 0;

        // Check correct answers
        foreach (var a in answers)
        {
            foreach (var ca in correctAnswers)
            {
                // Check key
                if (a.Keys.First() == ca.Keys.First())
                {
                    // Check value
                    string normalizedAnswer = a.Values.First().Trim().ToLower();
                    string normalizedCorrectAnswer = ca.Values.First().Trim().ToLower();
                    if (normalizedAnswer == normalizedCorrectAnswer)
                    {
                        correctCount++;
                    }
                }
            }
        }

        return correctCount == 0 ? 0 : mark / correctAnswers.Count * correctCount;
    }

    private float CalculateMatchingScore(SubmitPaperDetail detail, QuestionClone question, float mark)
    {
        // if detail.AnswerRaw is empty
        if (string.IsNullOrEmpty(detail.AnswerRaw))
        {
            return 0;
        }

        // Deserialize the AnswerRaw to get the user's answers
        var userAnswers = detail.AnswerRaw.Split('|')
                                          .Select(pair => pair.Split('_'))
                                          .ToDictionary(parts => parts[0], parts => parts[1]);

        // Initialize a counter for correct answers
        int correctCount = 0;

        // Iterate through each correct answer and check if the user's answer matches
        var correctAnswers = question.AnswerClones.FirstOrDefault();
        int numCorrectAnswers = 0;
        if (correctAnswers != null)
        {
            Dictionary<string, string> correctAnswersDict = new();

            // Deserialize the correct answers
            foreach (string answer in correctAnswers.Content!.Split('|'))
            {
                string[] parts = answer.Split('_');
                correctAnswersDict.Add(parts[0], parts[1]);
            }

            numCorrectAnswers = correctAnswersDict.Count;

            // Check if the user's answer matches the correct answer
            foreach (var userAnswer in userAnswers)
            {
                // ignore case, trim and compare
                if (correctAnswersDict.TryGetValue(userAnswer.Key, out string? correctAnswer) &&
                                       userAnswer.Value.Trim().Equals(correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    correctCount++;
                }
            }
        }

        // Calculate the score: total marks divided by the number of correct answers
        return correctCount == 0 ? 0 : mark / numCorrectAnswers * correctCount;
    }

    private float CalculateReadingScore(QuestionClone question, float mark, List<SubmitPaperDetail>? details = null)
    {
        // Reading
        float correctCount = 0;
        foreach (var qp in question.QuestionPassages)
        {
            var detail1 = details!.FirstOrDefault(x => x.QuestionId == qp.Id);
            if (detail1 != null)
            {
                bool isCorrect = true;
                foreach (var a in qp.AnswerClones)
                {
                    if (a.IsCorrect && !detail1.AnswerRaw!.Split('|').Contains(a.Id.ToString()))
                    {
                        isCorrect = false;
                        break;
                    }
                }

                if (isCorrect)
                {
                    correctCount += 1;
                }
            }
        }

        return correctCount == 0 ? 0 : mark / question.QuestionPassages.Count * correctCount;
    }

    private float CalculateWritingScore(SubmitPaperDetail detail)
    {
        // if answer is empty
        if (string.IsNullOrEmpty(detail.AnswerRaw))
        {
            return 0;
        }

        // if answer is nomal grade before
        return detail.Mark.HasValue && detail.Mark > 0 ? detail.Mark ?? 0 : 0;
    }

    public Task<List<SubmitPaper>> CalculateScorePaperAsync(DefaultIdType paperId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<DefaultIdType> SendLogAsync(SendLogRequest request, CancellationToken cancellationToken)
    {
        SubmitPaperLog spl = request.Adapt<SubmitPaperLog>();

        // get last submit paper log
        var lastLog = await _submitPaperLogRepository.FirstOrDefaultAsync(new SubmitPaperLogBySubmitPaperIdSpec(spl.SubmitPaperId), cancellationToken);

        // compare last log and current log
        bool flag = false;
        if (lastLog != null)
        {
            flag = lastLog.DeviceId != spl.DeviceId ||
                        lastLog.DeviceName != spl.DeviceName ||
                        lastLog.DeviceType != spl.DeviceType ||
                        lastLog.PublicIp != spl.PublicIp ||
                        lastLog.LocalIp != spl.LocalIp ||
                        lastLog.ProcessLog != spl.ProcessLog ||
                        lastLog.MouseLog != spl.MouseLog ||
                        lastLog.KeyboardLog != spl.KeyboardLog ||
                        lastLog.NetworkLog != spl.NetworkLog ||
                        lastLog.IsSuspicious != spl.IsSuspicious;
        }

        if (!flag && lastLog != null)
        {
            // Update last submit paper log lasmodified
            lastLog.LastModifiedOn = DateTime.Now;
            await _submitPaperLogRepository.UpdateAsync(lastLog, cancellationToken);
        }
        else
        {
            // Add new submit paper log
            _ = await _submitPaperLogRepository.AddAsync(spl, cancellationToken);
        }

        return spl.Id;
    }

    public bool IsCorrectAnswer(SubmitPaperDetail submitDetail, QuestionClone question, List<SubmitPaperDetail>? details = null)
    {
        float markFlag = 10f;
        float x = CalculateScore(submitDetail, question, markFlag, details);
        return x == markFlag;
    }

    public async Task<LastResultExamDto> GetLastResultExamAsync(Guid paperId, Guid userId, Guid submitPaperId, CancellationToken cancellationToken)
    {
        var currentUser = _currentUser.GetUserId();

        var paper = await _paperRepository.FirstOrDefaultAsync(new PaperByIdSpec(paperId), cancellationToken)
            ?? throw new NotFoundException(_t["Paper {0) Not Found."]);

        var spec = new ExamResultSpec(submitPaperId, paper, userId, currentUser);
        var submitPaper = await _submitPaperRepository.FirstOrDefaultAsync(spec, cancellationToken)
            ?? throw new NotFoundException(_t["SubmitPaper Not Found."]);

        if (userId == currentUser && !submitPaper.CheckDetailAnswerResult(paper.SubmitPapers.Count))
        {
            throw new BadRequestException(_t["You cannot view detail"]);
        }

        var student = await _userService.GetAsync(userId.ToString(), cancellationToken);

        float totalMark = 0;
        var submitPaperDetailsDtos = new List<SubmitPaperDetailDto>();
        foreach (var submit in submitPaper.SubmitPaperDetails)
        {
            float questionMark = paper.PaperQuestions.FirstOrDefault(x => x.QuestionId == submit.QuestionId)?.Mark ?? 0;

            bool isCorrect = false;
            // if question is Reading
            if (submit.Question.QuestionType == QuestionType.Reading)
            {
                // list of question passages detail
                List<SubmitPaperDetail> details = new List<SubmitPaperDetail>();
                foreach (var qp in submit.Question!.QuestionPassages)
                {
                    var detail1 = submitPaper.SubmitPaperDetails.FirstOrDefault(x => x.QuestionId == qp.Id);
                    if (detail1 != null)
                    {
                        details.Add(detail1);
                    }
                }

                totalMark += CalculateScore(submit, submit.Question!, paper.PaperQuestions.FirstOrDefault(x => x.QuestionId == submit.QuestionId).Mark, details);
                isCorrect = IsCorrectAnswer(submit, submit.Question, details);
            }
            else if (submit.Question.QuestionType == QuestionType.ReadingQuestionPassage)
            {
                var parentQuestion = paper.PaperQuestions.FirstOrDefault(x => x.QuestionId == submit.Question.QuestionParentId);
                totalMark += CalculateScore(submit, submit.Question!, parentQuestion.Mark / parentQuestion.Question.QuestionPassages.Count);
                isCorrect = IsCorrectAnswer(submit, submit.Question);
            }
            else
            {
                totalMark += CalculateScore(submit, submit.Question!, paper.PaperQuestions.FirstOrDefault(x => x.QuestionId == submit.QuestionId).Mark);
                isCorrect = IsCorrectAnswer(submit, submit.Question);
            }

            // Fill SubmitPaperDetailDto
            var submitPaperDetailDto = new SubmitPaperDetailDto
            {
                SubmitPaperId = submit.SubmitPaperId,
                QuestionId = submit.QuestionId,
                AnswerRaw = submit.AnswerRaw,
                Mark = submit.Mark,
                IsCorrect = isCorrect,
                CreatedBy = submit.CreatedBy,
                CreatedOn = submit.CreatedOn,
                LastModifiedBy = submit.LastModifiedBy,
                LastModifiedOn = submit.LastModifiedOn,
                Question = submit.Question.Adapt<QuestionDto>() // Assuming you have a mapping for QuestionDto
            };

            submitPaperDetailsDtos.Add(submitPaperDetailDto);
        }

        // Fill LastResultExamDto
        var examResultDto = new LastResultExamDto
        {
            Id = submitPaper.Id,
            PaperId = submitPaper.PaperId,
            Status = submitPaper.Status,
            StartTime = submitPaper.StartTime,
            EndTime = submitPaper.EndTime,
            TotalMark = submitPaper.TotalMark,
            TotalQuestion = submitPaper.SubmitPaperDetails.Count,
            Paper = paper.Adapt<PaperDto>(), // Assuming you have a mapping for PaperDto
            SubmitPaperDetails = submitPaperDetailsDtos,
            Student = student
        };

        return examResultDto;
    }
}
