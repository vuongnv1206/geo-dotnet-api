using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.Papers;
using FSH.WebApi.Application.Questions;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.TeacherGroup.PermissionClasses;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using FSH.WebApi.Domain.Class;
using FSH.WebApi.Domain.Examination;
using FSH.WebApi.Domain.Question;
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
        TypeAdapterConfig<QuestionFolder, QuestionTreeDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.Permission, src => src.Permissions)
            .Map(dest => dest.Children, src => src.Children);

        // Map Question to QuestionDto
        TypeAdapterConfig<Domain.Question.Question, QuestionDto>.NewConfig()
            .Map(dest => dest.QuestionPassages, src => src.QuestionPassages)
            .Map(dest => dest.QuestionFolder, src => src.QuestionFolder)
            .Map(dest => dest.Answers, src => src.Answers);

        TypeAdapterConfig<TeacherTeam, TeacherTeamDto>.NewConfig()
            .Map(dest => dest.TeacherPermissionInClassDto, src => src.TeacherPermissionInClasses);

        // GroupTeacher
        TypeAdapterConfig<GroupPermissionInClassDto, GroupPermissionInClass>.NewConfig()
           .Map(dest => dest.PermissionType, src => src.PermissionType);

        TypeAdapterConfig<PaperFolder, PaperFolderDto>.NewConfig()
           .Map(dest => dest.PaperFolderChildrens, src => src.PaperFolderChildrens);

        // UserClasses
        TypeAdapterConfig<UserClass, UserClassDto>.NewConfig()
            .Map(dest => dest.ClassesId, src => src.ClassesId)
            .Map(dest => dest.UserId, src => src.UserId);

        // Paper
        TypeAdapterConfig<Paper, PaperDto>.NewConfig()
          .Map(dest => dest.PaperFolder, src => src.PaperFolder)
          .Map(dest => dest.PaperLable, src => src.PaperLable);

        TypeAdapterConfig<CreateQuestionDto, Domain.Question.Question>.NewConfig()
            .Ignore(dest => dest.Answers)
            .Ignore(dest => dest.QuestionPassages)
            .TwoWays();

        TypeAdapterConfig<Answer, AnswerDto>.NewConfig();
    }
}