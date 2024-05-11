using FSH.WebApi.Domain.Question;

namespace FSH.WebApi.Application.Questions.Dtos;

public class QuestionFolderDto : IDto
{
    public string Name { get; private set; }
    public Guid? ParentId { get; private set; }
}