
using FSH.WebApi.Domain.Question.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.WebApi.Domain.Question;
public class QuestionClone : AuditableEntity, IAggregateRoot
{
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public virtual QuestionFolder? QuestionFolder { get; set; }
    public virtual QuestionType? QuestionType { get; set; }
    public Guid? QuestionLabelId { get; set; }
    [ForeignKey(nameof(QuestionLabelId))]
    public virtual QuestionLable? QuestionLabel { get; set; }
    public Guid? QuestionParentId { get; set; }
    public Guid? OriginalQuestionId { get; set; }
    [ForeignKey(nameof(OriginalQuestionId))]
    public virtual Question? OriginalQuestion { get; set; }
    public virtual QuestionClone? QuestionCloneParent { get; set; }
    public virtual List<QuestionClone> QuestionPassages { get; set; } = new();
    public virtual List<AnswerClone> AnswerClones { get; set; } = new();

    public QuestionClone()
    {

    }

    public QuestionClone(
               string? content,
                      string? image,
                             string? audio,
                                    Guid? questionFolderId,
                                           QuestionType? questionType,
                                                  Guid? questionLabelId)
    {
        Content = content;
        Image = image;
        Audio = audio;
        QuestionFolderId = questionFolderId;
        QuestionType = questionType;
        QuestionLabelId = questionLabelId;
    }

    public QuestionClone Update(
               string? content,
                      string? image,
                             string? audio,
                                    Guid? questionFolderId,
                                           QuestionType? questionType,
                                                  Guid? questionLabelId,
                                                  Guid?parentId)
    {
        Content = content;
        Image = image;
        Audio = audio;
        QuestionFolderId = questionFolderId;
        QuestionType = questionType;
        QuestionLabelId = questionLabelId;
        QuestionParentId = parentId;
        return this;
    }

    public void AddAnswerClones(List<AnswerClone> answerClones)
    {
        if (answerClones.Any())
            AnswerClones.AddRange(answerClones);
    }

    public bool CanDelete(DefaultIdType userId)
    {
        return CreatedBy == userId;
    }

    public void AddAnswerClone(AnswerClone answerClone)
    {
        AnswerClones.Add(answerClone);
    }

    public void RemoveAnswerClone(AnswerClone answerClone)
    {
        AnswerClones.Remove(answerClone);
    }

    public void AddQuestionPassage(QuestionClone questionClone)
    {
        QuestionPassages.Add(questionClone);
    }

    public void RemoveQuestionPassage(QuestionClone questionClone)
    {
        QuestionPassages.Remove(questionClone);
    }

    public void UpdateAnswerClones(List<AnswerClone> answerClones)
    {
        AnswerClones.Clear();
        foreach (var answer in answerClones)
        {
            var newAnswer = new AnswerClone(answer.Content, answer.IsCorrect);
            AddAnswerClone(newAnswer);
        }
    }

    public void UpdateQuestionPassages(List<QuestionClone> questionClones)
    {
        QuestionPassages = questionClones;
    }

    public bool CanUpdate(Guid userId)
    {
        return CreatedBy == userId;
    }

    public void RemoveAnswerClones()
    {
        AnswerClones.Clear();
    }
}
