using System.Security.Claims;
using FSH.WebApi.Application.Identity.Roles;
using FSH.WebApi.Application.Identity.Users;
using FSH.WebApi.Application.Common.Interfaces;
using FSH.WebApi.Domain.Question;
using Mapster;
using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Application.Questions.Specs;
namespace FSH.WebApi.Application.Questions;

public class GetFolderTreeRequest : IRequest<QuestionTreeDto>
{
    public Guid? ParentId { get; }

    public GetFolderTreeRequest(Guid? parentId)
    {
        ParentId = parentId;
    }
}

public class GetFolderTreeRequestHandler : IRequestHandler<GetFolderTreeRequest, QuestionTreeDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly IRepository<QuestionFolder> _questionFolderRepository;
    private readonly IStringLocalizer _t;

    public GetFolderTreeRequestHandler(ICurrentUser currentUser, IRepository<QuestionFolder> questionFolderRepository, IStringLocalizer<GetFolderTreeRequestHandler> localizer)
    {
        _currentUser = currentUser;
        _questionFolderRepository = questionFolderRepository;
        _t = localizer;
    }

    public async Task<QuestionTreeDto> Handle(GetFolderTreeRequest request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.GetUserId();

        var spec = new QuestionFoldersWithPermissionsSpecByUserId(userId, request.ParentId);
        var questionFolders = await _questionFolderRepository.ListAsync(spec, cancellationToken);
        var questionFolderTree = questionFolders.Adapt<List<QuestionTreeDto>>();

        if (!request.ParentId.HasValue)
        {
            return new QuestionTreeDto
            {
                Id = Guid.Empty,
                Name = "Root",
                CurrentShow = true,
                Children = questionFolderTree
            };
        }
        else
        {
            // get parent folder
            var parentFolder = await _questionFolderRepository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(request.ParentId.Value), cancellationToken);
            if (parentFolder == null)
            {
                throw new NotFoundException(_t["Folder {0} Not Found.", request.ParentId]);
            }

            var newTree = parentFolder.Adapt<QuestionTreeDto>();
            newTree.Children = questionFolderTree;
            newTree.CurrentShow = true;
            while (newTree.ParentId != null)
            {
                var parent = await _questionFolderRepository.FirstOrDefaultAsync(new QuestionFolderByIdSpec(newTree.ParentId.Value), cancellationToken);
                if (parent == null)
                {
                    throw new NotFoundException(_t["Folder {0} Not Found.", newTree.ParentId]);
                }

                var parentTree = parent.Adapt<QuestionTreeDto>();
                parentTree.Children = new List<QuestionTreeDto> { newTree };
                newTree = parentTree;
            }

            return newTree;
        }

    }
}
