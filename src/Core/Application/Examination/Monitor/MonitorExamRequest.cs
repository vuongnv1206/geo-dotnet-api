using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.PaperStudents.Dtos;
using FSH.WebApi.Application.Examination.SubmitPapers.Dtos;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using Mapster;

namespace FSH.WebApi.Application.Examination.Monitor;

public class MonitorExamRequest : PaginationFilter, IRequest<PaginationResponse<StudentMoni>>
{
    public DefaultIdType PaperId { get; set; }
}

public class MonitorExamRequestHandler : IRequestHandler<MonitorExamRequest, PaginationResponse<StudentMoni>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<Classes> _classRepo;
    private readonly IRepository<Paper> _paperRepository;
    private readonly IRepository<SubmitPaper> _submitPaperRepo;
    private readonly IRepository<SubmitPaperLog> _submitPaperLogRepository;

    private readonly IUserService _userService;
    public MonitorExamRequestHandler(
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IRepository<Paper> paperRepo,
        IUserService userService,
        IRepository<SubmitPaper> submitPaperRepo,
        IRepository<SubmitPaperLog> submitPaperLogRepository)
    {
        _currentUser = currentUser;
        _classRepo = classRepo;
        _paperRepository = paperRepo;
        _userService = userService;
        _submitPaperRepo = submitPaperRepo;
        _submitPaperLogRepository = submitPaperLogRepository;
    }

    public static bool IsSuspicious(SubmitPaperLog log)
    {
        // check is suspicious
        if (log.IsSuspicious == true)
        {
            return true;
        }

        // checl process log
        if (log.ProcessLog != null)
        {
            // TeamViewer, AnyDesk, Chrome Remote Desktop, UltraViewer, AnyDesk, Supremo, AeroAdmin, Ammyy Admin, Remote Utilities, Zoho Assist, Splashtop, LogMeIn, GoToMyPC, Join.me, WebEx, Zoom, Microsoft Teams, Skype, Slack, Discord
            // Zalo PC, Skype, Viber, Zalo, Facebook, Messenger, WhatsApp, Telegram, Skype, Viber
            List<string> suspiciousProcess = new()
            {
                "TeamViewer", "AnyDesk", "Chrome Remote Desktop", "UltraViewer", "AnyDesk", "Supremo", "AeroAdmin", "Ammyy Admin", "Remote Utilities", "Zoho Assist", "Splashtop", "LogMeIn", "GoToMyPC", "Join.me", "WebEx", "Zoom", "Microsoft Teams", "Skype", "Slack", "Discord",
                "Zalo PC", "Skype", "Viber", "Zalo", "Facebook", "Messenger", "WhatsApp", "Telegram", "Skype", "Viber"
            };

            foreach (string item in suspiciousProcess)
            {
                // ignore case
                if (log.ProcessLog.ToLower().Contains(item.ToLower()))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<PaginationResponse<StudentMoni>> Handle(MonitorExamRequest request, CancellationToken cancellationToken)
    {

        var spec = new PaperByIdWithAccessesSpec(request.PaperId);
        var paper = await _paperRepository.FirstOrDefaultAsync(spec, cancellationToken);

        List<StudentMoni> students = new();

        foreach (var item in paper.PaperAccesses)
        {
            if (item.UserId != null)
            {
                UserDetailsDto s = new UserDetailsDto();
                try
                {
                    s = await _userService.GetAsync(item.UserId.ToString(), cancellationToken);
                }
                catch (Exception)
                {
                    continue;
                }

                StudentMoni student = new()
                {
                    StudentId = (DefaultIdType)item.UserId,
                    Student = s,
                    Paper = paper.Adapt<PaperMoniDto>()
                };

                // Get submit paper by student id
                var spec2 = new SubmitPaperByStudentIdSpec(request.PaperId, item.UserId);
                var submitPaper = await _submitPaperRepo.FirstOrDefaultAsync(spec2, cancellationToken);

                if (submitPaper != null)
                {
                    student.PaperId = submitPaper.PaperId;
                    student.SubmitPaperId = submitPaper.Id;
                    student.CompletionStatus = (CompletionStatusEnum)submitPaper.Status;
                }
                else
                {
                    student.PaperId = request.PaperId;
                    student.SubmitPaperId = Guid.Empty;
                    student.CompletionStatus = CompletionStatusEnum.NotStarted;
                }

                students.Add(student);

            }
            else
            {
                if (item.ClassId != null)
                {
                    // get list student by class
                    var specclass = new StudentByClassIdSpec(item.ClassId);
                    var classpp = await _classRepo.FirstOrDefaultAsync(specclass, cancellationToken);
                    var studentIdsInclass = classpp.UserClasses.Select(x => x.Student.StId).ToList();
                    if (studentIdsInclass.Count > 0)
                    {
                        foreach (var studentId in studentIdsInclass)
                        {
                            UserDetailsDto s = new UserDetailsDto();
                            try
                            {
                                s = await _userService.GetAsync(studentId.ToString(), cancellationToken);
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                            StudentMoni student = new()
                            {
                                StudentId = (DefaultIdType)studentId,
                                Student = s,
                                ClassId = classpp.Id.ToString(),
                                Class = classpp.Adapt<ClassMoniDto>(),
                                Paper = paper.Adapt<PaperMoniDto>()
                            };

                            // Get submit paper by student id
                            var spec2 = new SubmitPaperByStudentIdSpec(request.PaperId, studentId);
                            var submitPaper = await _submitPaperRepo.FirstOrDefaultAsync(spec2, cancellationToken);

                            if (submitPaper != null)
                            {
                                student.PaperId = submitPaper.PaperId;
                                student.SubmitPaperId = submitPaper.Id;
                                student.SubmitPaper = submitPaper.Adapt<SubmitPaperDto>();
                                student.CompletionStatus = (CompletionStatusEnum)submitPaper.Status;
                            }
                            else
                            {
                                student.PaperId = request.PaperId;
                                student.SubmitPaperId = Guid.Empty;
                                student.CompletionStatus = CompletionStatusEnum.NotStarted;
                            }

                            students.Add(student);
                        }
                    }
                }
            }
        }

        // Get submit paper logs
        foreach (var student in students)
        {
            try
            {
                var spec3 = new SubmitPaperLogBySubmitPaperIdSpec(student.SubmitPaperId);
                var logs = await _submitPaperLogRepository.ListAsync(spec3, cancellationToken);
                foreach (var log in logs)
                {
                    if (log.IsSuspicious == true)
                    {
                        student.IsSuspicious = true;
                        break;
                    }

                    if (IsSuspicious(log))
                    {
                        student.IsSuspicious = true;
                        break;
                    }

                }
            }
            catch (Exception)
            {
            }
        }

        var res = new PaginationResponse<StudentMoni>(students, students.Count, request.PageNumber, request.PageSize);
        return res;
    }
}

public class SubmitPaperLogBySubmitPaperIdSpec : Specification<SubmitPaperLog>, ISingleResultSpecification
{
    public SubmitPaperLogBySubmitPaperIdSpec(DefaultIdType? submitPaperId)
    {
        _ = Query.Where(p => p.SubmitPaperId == submitPaperId).OrderByDescending(x => x.CreatedOn);
    }

    public SubmitPaperLogBySubmitPaperIdSpec(DefaultIdType? submitPaperId, int pageNumber, int pageSize)
        : this(submitPaperId)
    {
        _ = Query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}