using FSH.WebApi.Application.Assignments;
using FSH.WebApi.Application.Assignments.Dtos;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Class.Comments.Dto;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Application.Class.UserClasses.Dto;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Examination.Papers.Dtos;
using FSH.WebApi.Application.Examination.PaperStatistics;
using FSH.WebApi.Application.Examination.PaperStudents.Dtos;
using FSH.WebApi.Application.Examination.Reviews;
using FSH.WebApi.Application.Examination.Services.Models;
using FSH.WebApi.Application.Examination.SubmitPapers;
using FSH.WebApi.Application.Extensions;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.TeacherGroup.GroupTeachers;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.Assignment;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using FSH.WebApi.Domain.TeacherGroup;
using Mapster;

namespace FSH.WebApi.Infrastructure.Mapping;

public class MapsterSettings
{
    public static void Configure()
    {
        // here we will define the type conversion / Custom-mapping
        // More details at https://github.com/MapsterMapper/Mapster/wiki/Custom-mapping

        // This one is actually not necessary as it's mapped by convention
        // TypeAdapterConfig<Product, ProductDto>.NewConfig().Map(dest => dest.BrandName, src => src.Brand.Name);

        // Map QuestionFolder to QuestionTreeDto
        _ = TypeAdapterConfig<QuestionFolder, QuestionTreeDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.Permission, src => src.Permissions)
            .Map(dest => dest.Children, src => src.Children);

        // Map Question to QuestionDto
        _ = TypeAdapterConfig<Domain.Question.Question, QuestionDto>.NewConfig()
            .Map(dest => dest.QuestionPassages, src => src.QuestionPassages)
            .Map(dest => dest.QuestionFolder, src => src.QuestionFolder)
            .Map(dest => dest.Answers, src => src.Answers);

        _ = TypeAdapterConfig<CreateQuestionDto, NewQuestionDto>.NewConfig();
        _ = TypeAdapterConfig<CreateQuestionDto, Domain.Question.Question>.NewConfig()
            .Ignore(dest => dest.Answers)
            .Ignore(dest => dest.QuestionPassages)
            .TwoWays();

        _ = TypeAdapterConfig<CreateQuestionCloneDto, Domain.Question.Question>.NewConfig()
          .Map(dest => dest.Answers, src => src.Answers)
            .Map(dest => dest.QuestionPassages, src => src.QuestionPassages.Adapt<List<CreateQuestionCloneDto>>())
          .TwoWays();

        _ = TypeAdapterConfig<CreateQuestionCloneDto, Domain.Question.QuestionClone>.NewConfig()
        .Map(dest => dest.AnswerClones, src => src.Answers)
        .Map(dest => dest.QuestionPassages, src => src.QuestionPassages.Adapt<List<CreateQuestionCloneDto>>())
        .TwoWays();

        _ = TypeAdapterConfig<CreateQuestionDto, Domain.Question.Question>.NewConfig()
          .Ignore(dest => dest.Answers)
          .Ignore(dest => dest.QuestionPassages)
          .TwoWays();

        _ = TypeAdapterConfig<CreateQuestionDto, QuestionClone>.NewConfig()
            .Ignore(dest => dest.AnswerClones)
            .Ignore(dest => dest.QuestionPassages)
            .Ignore(dest => dest.QuestionFolder)
            .Ignore(dest => dest.QuestionLabel)
            .TwoWays();

        _ = TypeAdapterConfig<Domain.Question.Question, QuestionClone>.NewConfig()
            .Ignore(dest => dest.QuestionLabel)
            .Ignore(dest => dest.QuestionFolder)
            .Map(dest => dest.QuestionLabelId, src => src.QuestionLableId)
            .Map(dest => dest.QuestionPassages, src => src.QuestionPassages)
            .Map(dest => dest.QuestionFolder, src => src.QuestionFolder)
            .Map(dest => dest.AnswerClones, src => src.Answers);

        _ = TypeAdapterConfig<Domain.Question.Question, QuestionForStudentDto>.NewConfig()
            .Map(dest => dest.QuestionPassages, src => src.QuestionPassages)
            .Map(dest => dest.Answers, src => CustomMappingExtensions.MapAnswers(src.Answers));

        _ = TypeAdapterConfig<Domain.Question.Question, QuestionPassagesForStudentDto>.NewConfig()
            .Map(dest => dest.Answers, src => CustomMappingExtensions.MapAnswers(src.Answers));

        _ = TypeAdapterConfig<Answer, AnswerForStudentDto>.NewConfig()
            .Map(dest => dest.IsCorrect, src => false);

        _ = TypeAdapterConfig<Paper, PaperForStudentDto>.NewConfig()
            .Map(dest => dest.PaperFolder, src => src.PaperFolder)
            .Map(dest => dest.PaperLable, src => src.PaperLabel)
            .Map(dest => dest.Questions, src => CustomMappingExtensions.MapQuestionsForStudent(src.PaperQuestions))
            .Map(dest => dest.TotalAttended, src => src.SubmitPapers.Count())
            .Map(dest => dest.NumberOfQuestion, src => src.PaperQuestions.Count());

        _ = TypeAdapterConfig<Answer, CreateAnswerDto>.NewConfig();
        _ = TypeAdapterConfig<AnswerClone, CreateAnswerDto>.NewConfig();

        _ = TypeAdapterConfig<Answer, AnswerDto>.NewConfig();

        _ = TypeAdapterConfig<AnswerClone, AnswerDto>.NewConfig()
             .Map(dest => dest.QuestionId, src => src.QuestionCloneId);

        _ = TypeAdapterConfig<QuestionClone, QuestionDto>.NewConfig()
           .Map(dest => dest.QuestionLable, src => src.QuestionLabel)

    // .Map(dest => dest.QuestionPassages, src => src.QuestionPassages)
    .Map(dest => dest.QuestionPassages, src => src.QuestionPassages.Adapt<List<QuestionDto>>())
           .Map(dest => dest.QuestionFolder, src => src.QuestionFolder)
           .Map(dest => dest.Answers, src => src.AnswerClones);

        _ = TypeAdapterConfig<QuestionClone, QuestionForStudentDto>.NewConfig()
           .Map(dest => dest.QuestionPassages, src => src.QuestionPassages)
           .Map(dest => dest.Answers, src => src.AnswerClones);

        _ = TypeAdapterConfig<AnswerClone, AnswerForStudentDto>.NewConfig()
           .Map(dest => dest.IsCorrect, src => false)
           .Map(dest => dest.Content, src => (src.QuestionClone.QuestionType == QuestionType.FillBlank || src.QuestionClone.QuestionType == QuestionType.Matching) ? string.Empty : src.Content);

        _ = TypeAdapterConfig<TeacherTeam, TeacherTeamDto>.NewConfig()
            .Map(dest => dest.TeacherPermissionInClassDto, src => src.TeacherPermissionInClasses);

        // GroupTeacher
        _ = TypeAdapterConfig<GroupPermissionInClassDto, GroupPermissionInClass>.NewConfig()
           .Map(dest => dest.PermissionType, src => src.PermissionType);

        _ = TypeAdapterConfig<GroupTeacher, GroupTeacherDto>.NewConfig()
         .Map(dest => dest.GroupPermissionInClasses, src => src.GroupPermissionInClasses)
         .Map(dest => dest.TeacherTeams, src => src.TeacherInGroups.Select(tig => tig.TeacherTeam));

        // Paper Folder
        _ = TypeAdapterConfig<PaperFolder, PaperFolderDto>.NewConfig()
           .Map(dest => dest.PaperFolderChildrens, src => src.PaperFolderChildrens.Adapt<List<PaperFolderDto>>());

        _ = TypeAdapterConfig<PaperFolder, PaperFolderParentDto>.NewConfig();

        // UserClasses
        _ = TypeAdapterConfig<UserClass, UserClassDto>.NewConfig()
            .Map(dest => dest.ClassesId, src => src.ClassesId)
            .Map(dest => dest.StudentId, src => src.StudentId);

        // Paper
        _ = TypeAdapterConfig<Paper, PaperDto>.NewConfig()
          .Map(dest => dest.PaperFolder, src => src.PaperFolder)
          .Map(dest => dest.PaperLable, src => src.PaperLabel)
          .Map(dest => dest.Questions, src => CustomMappingExtensions.MapQuestions(src.PaperQuestions))
          .Map(dest => dest.TotalAttended, src => src.SubmitPapers.Count())
          .Map(dest => dest.NumberOfQuestion, src => src.PaperQuestions.Count());

        _ = TypeAdapterConfig<Paper, PaperStudentDto>.NewConfig()
          .Map(dest => dest.TotalAttended, src => src.SubmitPapers.Count())
          .Map(dest => dest.NumberOfQuestion, src => src.PaperQuestions.Count());

        _ = TypeAdapterConfig<Paper, StudentTestDto>.NewConfig()
            .Map(dest => dest.PaperLabelName, src => src.PaperLabel.Name)
            .Map(dest => dest.SubjectName, src => src.Subject.Name);

        _ = TypeAdapterConfig<SubmitPaper, StudentTestHistoryDto>.NewConfig()
            .Map(dest => dest.PaperLabelName, src => src.Paper.PaperLabel.Name)
            .Map(dest => dest.PaperLabelId, src => src.Paper.PaperLabelId)
            .Map(dest => dest.SubjectId, src => src.Paper.SubjectId)
            .Map(dest => dest.SubjectName, src => src.Paper.Subject.Name)
            .Map(dest => dest.Duration, src => src.Paper.Duration)
            .Map(dest => dest.ExamName, src => src.Paper.ExamName)
            .Map(dest => dest.StartedTime, src => src.StartTime)
            .Map(dest => dest.SubmittedTime, src => src.EndTime)
            .Map(dest => dest.CompletionStatus, src => src.Status)
            .Map(dest => dest.Score, src => src.getScore())
            .Map(dest => dest.ShowMarkResult, src => src.Paper.ShowMarkResult);

        _ = TypeAdapterConfig<PaperQuestion, CreateUpdateQuestionInPaperDto>.NewConfig();

        _ = TypeAdapterConfig<SubmitPaper, LastResultExamDto>.NewConfig()
               .Map(dest => dest.Paper, src => src.Paper)
               .Map(dest => dest.TotalQuestion, src => src.Paper.PaperQuestions.Count());

        _ = TypeAdapterConfig<SubmitPaperDetail, SubmitPaperDetailDto>.NewConfig()
           .Map(dest => dest.IsCorrect, src => src.IsAnswerCorrect(src.Question, src.Question.AnswerClones.ToList()));

        _ = TypeAdapterConfig<SubmitPaperLog, SendLogRequest>.NewConfig();

        _ = TypeAdapterConfig<PaperAccess, PaperAccessDto>.NewConfig();

        _ = TypeAdapterConfig<Answer, CreateAnswerDto>.NewConfig();

        // Post
        _ = TypeAdapterConfig<Post, PostDto>.NewConfig()
            .Map(dest => dest.NumberLikeInThePost, src => src.PostLikes.Count());

        // Comment
        _ = TypeAdapterConfig<Comment, CommentDto>.NewConfig()
            .Map(dest => dest.NumberLikeInComment, src => src.CommentLikes.Count());

        // Class
        _ = TypeAdapterConfig<Classes, ClassDto>.NewConfig()
          .Map(dest => dest.NumberUserOfClass, src => src.UserClasses.Count())
          .Map(dest => dest.Students, src => src.UserClasses.Select(p => p.Student))
          .Map(dest => dest.Assignments, src => src.AssignmentClasses.Select(pq => pq.Assignment))
          .Map(dest => dest.Papers, src => src.PaperAccesses.Select(pq => pq.Paper));

        _ = TypeAdapterConfig<Classes, ClassViewListDto>.NewConfig()
          .Map(dest => dest.NumberUserOfClass, src => src.UserClasses.Count());

        _ = TypeAdapterConfig<Assignment, AssignmentDetailsDto>.NewConfig()
                .Map(dest => dest.ClassesId, src => src.AssignmentClasses.Select(pq => pq.ClassesId));

        _ = TypeAdapterConfig<Assignment, AssignmentDto>.NewConfig();
        _ = TypeAdapterConfig<Assignment, SubmissionAssignmentDto>.NewConfig()
            .Map(dest => dest.Student, src => src.AssignmentStudents.Select(pq => pq.Student));

        _ = TypeAdapterConfig<PaperPermission, PaperPermissionDto>.NewConfig();
        _ = TypeAdapterConfig<PaperFolderPermission, PaperFolderPermissionDto>.NewConfig();

        // Examination.Services.Models
        _ = TypeAdapterConfig<QuestionModel, QuestionDto>.NewConfig()
            .Map(dest => dest.Answers, src => src.Answers)
            .Map(dest => dest.QuestionPassages, src => src.QuestionPassages);
        _ = TypeAdapterConfig<QuestionPassageModel, QuestionPassagesDto>.NewConfig();
        _ = TypeAdapterConfig<AnswerModel, AnswerDto>.NewConfig();

        // paper statistic
        _ = TypeAdapterConfig<Paper, PaperInfoStatistic>.NewConfig()
            .Map(dest => dest.TotalAttendee, src => src.SubmitPapers.GroupBy(x => x.CreatedBy).Count())
            .Map(dest => dest.TotalRegister, src => src.SubmitPapers.GroupBy(x => x.CreatedBy).Count())
            .Map(dest => dest.MarkPopular, src =>
                src.SubmitPapers.Where(x => x.Status == SubmitPaperStatus.End)
                                .GroupBy(x => Math.Ceiling(x.TotalMark))
                                .OrderByDescending(g => g.Count())
                                .Select(g => g.Key)
                                .FirstOrDefault())
            .Map(dest => dest.TotalPopular, src =>
                src.SubmitPapers.Where(x => x.Status == SubmitPaperStatus.End)
                                .GroupBy(x => Math.Ceiling(x.TotalMark))
                                .OrderByDescending(g => g.Count())
                                .Select(g => g.Count())
                                .FirstOrDefault())
            .Map(dest => dest.TotalDoing, src => src.SubmitPapers.Where(x => x.Status == SubmitPaperStatus.Doing).Count())
            .Map(dest => dest.AverageMark, src => src.SubmitPapers.Sum(x => x.TotalMark)
            / src.SubmitPapers.Where(sb => sb.Status == SubmitPaperStatus.End).Count());
    }
}