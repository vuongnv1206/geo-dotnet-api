using FSH.WebApi.Domain.Question.Enums;
using System.ComponentModel;

namespace FSH.WebApi.Domain.Question;

public class Question : AuditableEntity, IAggregateRoot
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public virtual QuestionFolder? QuestionFolder { get; set; }
    public virtual QuestionType? QuestionType { get; set; }
    public Guid? QuestionLableId { get; set; }
    public virtual QuestionLable? QuestionLable { get; set; }
    public Guid? ParentId { get; set; }
    public virtual Question? QuestionParent { get; set; }
    public virtual List<Question> QuestionPassages { get; set; } = new();
    public virtual List<Answer> Answers { get; set; } = new();

    public Question()
    {

    }

    public Question(
        string? content,
        string? image,
        string? audio,
        Guid? questionFolderId,
        QuestionType? questionType,
        Guid? questionLableId)
    {
        Content = content;
        Image = image;
        Audio = audio;
        QuestionFolderId = questionFolderId;
        QuestionType = questionType;
        QuestionLableId = questionLableId;
    }

    public void AddAnswers(List<Answer> answers)
    {

        foreach (var answer in answers)
        {
            Answers.Add(answer);
        }
    }

    public bool CanDelete(DefaultIdType userId)
    {
        return CreatedBy == userId;
    }
    
    public void AddAnswer(Answer answer)
    {
        Answers.Add(answer);
    }

     public void AddPassage(Question passage)
    {
        QuestionPassages.Add(passage);
    }

    public void UpdateAnswers(List<Answer> answers)
    {
        Answers.RemoveAll(a => !answers.Any(x => x.Id == a.Id));

        foreach (var answer in answers)
        {
            var existingAnswer = Answers.FirstOrDefault(a => a.Id == answer.Id);
            if (existingAnswer != null)
            {
                existingAnswer.Update(answer.Content, answer.IsCorrect,answer.QuestionId);
            }
            else
            {
                AddAnswer(answer);
            }
        }
    }

    public Question Update(string? content, string? image, string? audio, Guid? questionFolderId, QuestionType? questionType, Guid? questionLableId, Guid? parentId)
    {
        Content = content;
        Image = image;
        Audio = audio;
        QuestionFolderId = questionFolderId;
        QuestionType = questionType;
        QuestionLableId = questionLableId;
        ParentId = parentId;

        return this;
    }

    public bool CanUpdate(Guid userId)
    {
        return CreatedBy == userId;
    }
}