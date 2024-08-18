using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Application.Class.UserStudents;
using FSH.WebApi.Application.TeacherGroup.TeacherTeams;
using Microsoft.AspNetCore.Mvc;
using FSH.WebApi.Application.Class.UserStudents.Dto;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class;
using FSH.WebApi.Application.Examination.PaperStatistics;

namespace FSH.WebApi.Host.Controllers.Class;
public class StudentController : VersionedApiController
{

    [HttpPost("search")]
    [MustHavePermission(FSHAction.Search, FSHResource.Classes)]
    [OpenApiOperation("Search student using available filters.", "")]
    public Task<PaginationResponse<UserStudentDto>> SearchAsync(SearchStudentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(FSHAction.View, FSHResource.Classes)]
    [OpenApiOperation("Get student details.", "")]
    public Task<UserStudentDto> GetAsync(Guid id)
    {
        return Mediator.Send(new GetStudentDetailRequest(id));
    }

    [HttpPost]
    [MustHavePermission(FSHAction.Create, FSHResource.Classes)]
    [OpenApiOperation("Create a student", "")]
    public Task AddStudentInClass(CreateStudentRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Classes)]
    [OpenApiOperation("Update a Student.", "")]
    public async Task<ActionResult<Guid>> UpdateStudentRegistrationStatusAsync(UpdateStudentWhenUserRegisterRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(FSHAction.Delete, FSHResource.Classes)]
    [OpenApiOperation("Delete a Student.", "")]
    public Task DeleteUserInClass(Guid id)
    {
        return Mediator.Send(new DeleteStudentRequest(id));
    }

    [HttpPut("student-in-class/{id:guid}")]
    [MustHavePermission(FSHAction.Update, FSHResource.Classes)]
    [OpenApiOperation("Update student information.", "")]
    public async Task<ActionResult<Guid>> UpdateInformationStudentInClass(UpdateInformationStudentRequest request, Guid id)
    {
        return id != request.Id
            ? BadRequest()
            : Ok(await Mediator.Send(request));
    }

    [HttpGet("format-import-student-excel")]
    [MustHavePermission(FSHAction.Create, FSHResource.Classes)]
    [OpenApiOperation("get template import student", "")]
    public async Task<IActionResult> GetTemplateAddStudentExcel()
    {
        var fileBytes = await Mediator.Send(new FormatAddStudentExcelRequest());
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ImportStudent.xlsx");
    }

    [HttpPost("import-student-excel")]
    [MustHavePermission(FSHAction.Create, FSHResource.Classes)]
    [OpenApiOperation("Import student to class using excel", "")]
    public async Task<IActionResult> ImportTemplateStudentExcel(IFormFile formFile, Guid classId)
    {
        var request = new ImportStudentExcelRequest(classId, formFile);
        return Ok(await Mediator.Send(request));
    }

}
