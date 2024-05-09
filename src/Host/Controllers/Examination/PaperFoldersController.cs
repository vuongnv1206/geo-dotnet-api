using FSH.WebApi.Application.Examination.PaperFolders;
using FSH.WebApi.Application.Examination.PaperFolders.Dtos;
using FSH.WebApi.Host.Controllers.Question;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperFoldersController : VersionedApiController
{
    [HttpPost("search")]
    [MustHavePermission(FSHAction.View, FSHResource.GroupTeachers)]
    [OpenApiOperation("Search paper folder using available filder", "")]
    public Task<List<PaperFolderDto>> SearchAsync(SearchPaperFolderRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [OpenApiOperation("Get paper folder detail.")]
    public Task<PaperFolderDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetPaperFolderRequest(id));
    }

    [HttpPut("{id:guid}")]
    [OpenApiOperation("Update information of paper folder.")]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdatePaperFolderRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }
}
