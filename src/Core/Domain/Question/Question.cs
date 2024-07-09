using FSH.WebApi.Domain.Question.Enums;

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
    public Guid? QuestionParentId { get; set; }
    public virtual Question? QuestionParent { get; set; }
    public virtual List<Question> QuestionPassages { get; set; } = new();
    public virtual List<Answer> Answers { get; set; } = new();
    public QuestionStatus? QuestionStatus { get; set; }

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
        if (answers.Any())
            Answers.AddRange(answers);
    }

    public bool CanDelete(DefaultIdType userId)
    {
        return CreatedBy == userId;
    }

    public void AddAnswer(Answer answer)
    {
        Answers.Add(answer);
    }

    public void RemoveAnswer(Answer answer)
    {
        Answers.Remove(answer);
    }

    public void UpdateAnswers(List<Answer> answers)
    {
        Answers.Clear();
        foreach (var answer in answers)
        {
            var newAnswer = new Answer(answer.Content, answer.IsCorrect);
            AddAnswer(newAnswer);
        }
    }

    public Question Update(string? content, string? image, string? audio, Guid? questionFolderId, QuestionType? questionType, Guid? questionLableId, Guid? parentId)
    {
        if(content != null && !content.Equals(Content)) Content = content;
        if(image != null && !image.Equals(Image)) Image = image;
        if(audio != null && !audio.Equals(Audio)) Audio = audio;
        if(questionFolderId.HasValue && questionFolderId.Value != Guid.Empty && !questionFolderId.Equals(QuestionFolderId)) QuestionFolderId = questionFolderId;
        if(questionType != null && !questionType.Equals(QuestionType)) QuestionType = questionType;
        if(questionLableId.HasValue && questionLableId.Value != Guid.Empty && !questionLableId.Equals(QuestionLableId)) QuestionLableId = questionLableId;
        if(parentId.HasValue && parentId.Value != Guid.Empty && !parentId.Equals(QuestionParentId)) QuestionParentId = parentId;

        return this;
    }

    public bool CanUpdate(Guid userId)
    {
        return CreatedBy == userId;
    }

    public void RemoveAnswers()
    {
        Answers.Clear();
    }

    public void AddPassage(Question newPassage)
    {
        QuestionPassages.Add(newPassage);
    }
}