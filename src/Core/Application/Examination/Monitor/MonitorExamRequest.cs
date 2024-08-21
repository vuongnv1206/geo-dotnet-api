using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Examination.Papers;
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

    private readonly IUserService _userService;
    public MonitorExamRequestHandler(
        ICurrentUser currentUser,
        IRepository<Classes> classRepo,
        IRepository<Paper> paperRepo,
        IUserService userService,
        IRepository<SubmitPaper> submitPaperRepo)
    {
        _currentUser = currentUser;
        _classRepo = classRepo;
        _paperRepository = paperRepo;
        _userService = userService;
        _submitPaperRepo = submitPaperRepo;
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
                StudentMoni student = new()
                {
                    StudentId = (DefaultIdType)item.UserId,
                    Student = await _userService.GetAsync(item.UserId.ToString(), cancellationToken)
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
                            StudentMoni student = new()
                            {
                                StudentId = (DefaultIdType)studentId,
                                Student = await _userService.GetAsync(studentId.ToString(), cancellationToken),
                                ClassId = classpp.Id.ToString(),
                                Class = classpp.Adapt<ClassMoniDto>()
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

        var res = new PaginationResponse<StudentMoni>(students, students.Count, request.PageNumber, request.PageSize);
        return res;
    }
}
