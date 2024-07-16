

using FSH.WebApi.Application.Questions.Specs;
using FSH.WebApi.Domain.Question;
using FSH.WebApi.Domain.Question.Enums;
using Mapster;

namespace FSH.WebApi.Application.Questions;
public class UpdateQuestionCloneRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public string? Image { get; set; }
    public string? Audio { get; set; }
    public Guid? QuestionFolderId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public Guid? QuestionLabelId { get; set; }
    public Guid? ParentId { get; set; }
    public List<CreateQuestionDto>? QuestionPassages { get; set; }
    public List<CreateAnswerDto>? Answers { get; set; }
}

public class UpdateQuestionCloneRequestValidator : CustomValidator<UpdateQuestionCloneRequest>
{
    public UpdateQuestionCloneRequestValidator(
               IReadRepository<QuestionFolder> _folderRepo,
                      IReadRepository<QuestionLable> _labelRepo,
                             IStringLocalizer<UpdateQuestionCloneRequestValidator> _t)
    {
        RuleFor(x => x.QuestionFolderId)
            .MustAsync(async (folderId, ct) => !folderId.HasValue || await _folderRepo.FirstOrDefaultAsync(new QuestionFolderByIdSpec(folderId), ct) is not null)
            .WithMessage((_, folderId) => _t["Folder {0} Not Found.", folderId]);
        RuleFor(x => x.QuestionLabelId)
            .MustAsync(async (labelId, ct) => !labelId.HasValue || await _labelRepo.FirstOrDefaultAsync(new QuestionLabelByIdSpec(labelId), ct) is not null)
            .WithMessage((_, labelId) => _t["Question Label {0} Not Found.", labelId]);
    }



    public class UpdateQuestionCloneRequestHandler : IRequestHandler<UpdateQuestionCloneRequest, Guid>
    {
        private readonly IRepositoryWithEvents<QuestionClone> _questionRepo;
        private readonly IStringLocalizer _t;
        private readonly ICurrentUser _currentUser;
        private readonly IRepositoryWithEvents<AnswerClone> _answerRepo;
        private readonly IRepositoryWithEvents<QuestionFolder> _questionFolderRepository;

        public UpdateQuestionCloneRequestHandler(
                       IRepositoryWithEvents<QuestionClone> questionRepo,
                                  IStringLocalizer<UpdateQuestionCloneRequestHandler> t,
                                             ICurrentUser currentUser,
                                                        IRepositoryWithEvents<AnswerClone> answerRepo,
                                                                   IRepositoryWithEvents<QuestionFolder> questionFolderRepository)
        {
            _questionRepo = questionRepo;
            _t = t;
            _currentUser = currentUser;
            _answerRepo = answerRepo;
            _questionFolderRepository = questionFolderRepository;
        }



        public async Task<DefaultIdType> Handle(UpdateQuestionCloneRequest request, CancellationToken cancellationToken)
        {
            var question = await _questionRepo.FirstOrDefaultAsync(new QuestionCloneByIdSpec(request.Id));
            _ = question ?? throw new NotFoundException(_t["Question {0} Not Found.", request.Id]);

            if (!question.CanUpdate(_currentUser.GetUserId()))
            {
                var folder = await _questionFolderRepository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(question.QuestionFolderId));
                if (folder.CreatedBy != _currentUser.GetUserId())
                {
                    throw new ForbiddenException(_t["You do not have permission to update this question."]);
                }
            }

            question.Update(request.Content, request.Image, request.Audio, request.QuestionFolderId, request.QuestionType, request.QuestionLabelId, request.ParentId);
            foreach (var answer in question.AnswerClones)
            {
                await _answerRepo.DeleteAsync(answer);
            }

            foreach (var passage in question.QuestionPassages)
            {
                await _questionRepo.DeleteAsync(passage);
            }

            question.AnswerClones.Clear();
            question.QuestionPassages.Clear();
            if (request.Answers != null)
            {
                foreach (var newAnswer in request.Answers.Adapt<List<AnswerClone>>())
                {
                    if (!newAnswer.Content.Equals(string.Empty))
                    {
                        question.AddAnswerClone(newAnswer);
                    }
                }
            }

            // validate question by question type
            // Multiple choice
            if (question.QuestionType == QuestionType.MultipleChoice)
            {
                if (question.AnswerClones.Count < 3)
                    throw new BadRequestException(_t["Multiple choice question must have at least 3 answers."]);
                if (question.AnswerClones.Count(a => a.IsCorrect) < 2)
                    throw new BadRequestException(_t["Multiple choice question must have at least 2 correct answer."]);
            }

            // Single choice
            if (question.QuestionType == QuestionType.SingleChoice)
            {
                if (question.AnswerClones.Count < 2)
                    throw new BadRequestException(_t["Single choice question must have at least 2 answers."]);
                if (question.AnswerClones.Count(a => a.IsCorrect) > 1)
                    throw new BadRequestException(_t["Single choice question must have exactly 1 correct answer."]);
            }

            // Matching
            if (question.QuestionType == QuestionType.Matching)
            {
                if (question.AnswerClones.Count < 1)
                    throw new BadRequestException(_t["Matching question must have at least 1 pair."]);
            }

            // Fill in the blank
            if (question.QuestionType == QuestionType.FillBlank)
            {
                int blankCount = question.Content.Split("$_fillblank").Length - 1;
                if (question.AnswerClones.Count != blankCount)
                    throw new BadRequestException(_t["Fill in the blank question must have exactly {0} answers.", blankCount]);

            }

            // Writing
            if (question.QuestionType == QuestionType.Writing)
            {
                if (question.Content.Equals(string.Empty))
                    throw new BadRequestException(_t["Writing question must have content."]);
            }

            // Reading question passage
            if (question.QuestionType == QuestionType.Reading)
            {
                var questionPassages = request.QuestionPassages;
                if (questionPassages == null)
                    throw new BadRequestException(_t["Reading question passage must have at least 1 question passage."]);
                if (questionPassages.Count < 1)
                    throw new BadRequestException(_t["Reading question passage must have at least 1 question passage."]);
                // check answers of question passages
                foreach (var passage in questionPassages)
                {
                    if (passage.Answers == null)
                        throw new BadRequestException(_t["Reading question passage must have at least 1 answer."]);
                    if (passage.Answers.Count < 2)
                        throw new BadRequestException(_t["Reading question passage must have at least 2 answer."]);
                    if (passage.Answers.Count(a => a.IsCorrect) < 1)
                        throw new BadRequestException(_t["Reading question passage must have at least 1 correct answer."]);
                }
            }

            await _questionRepo.UpdateAsync(question);

            if (request.QuestionPassages != null)
            {
                await AddQuestionPassages(request.QuestionPassages, question.Id, cancellationToken);
            }

            return question.Id;

        }

        private async Task AddQuestionPassages(List<CreateQuestionDto> passages, Guid parentId, CancellationToken cancellationToken)
        {

            foreach (var passageDto in passages)
            {
                passageDto.QuestionParentId = parentId;
                var passage = passageDto.Adapt<QuestionClone>();
                var answers = passageDto.Answers?.Adapt<List<AnswerClone>>();

                if (answers != null)
                {
                    passage.AddAnswerClones(answers);
                }

                passage.QuestionType = QuestionType.ReadingQuestionPassage;
                await _questionRepo.AddAsync(passage, cancellationToken);

                if (passageDto.QuestionPassages != null)
                {
                    await AddQuestionPassages(passageDto.QuestionPassages, passage.Id, cancellationToken);
                }
            }
        }
    }

}
