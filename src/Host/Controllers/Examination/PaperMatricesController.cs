using FSH.WebApi.Application.Examination.Matrices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace FSH.WebApi.Host.Controllers.Examination;
public class PaperMatricesController  : VersionedApiController
{
    /// <remarks>
    /// Sample request:
    /// 
    ///     {
    ///         "Name": "Sample Matrix",
    ///         "Content": [
    ///             {
    ///                 \"QuestionFolderId\": \"550e8400-e29b-41d4-a716-446655440000\",
    ///                 \"CriteriaQuestions\": [
    ///                     {
    ///                         \"QuestionLabelId\": \"650e8400-e29b-41d4-a716-446655440000\",
    ///                         \"QuestionType\": 1,
    ///                         \"NumberOfQuestion\": 3,
    ///                         \"RawIndex": "1,2,3\"
    ///                     },
    ///                     {
    ///                         \"QuestionLabelId\": \"750e8400-e29b-41d4-a716-446655440000\",
    ///                         \"QuestionType\": 2,
    ///                         \"NumberOfQuestion\": 2,
    ///                         \"RawIndex\": \"4,5\"
    ///                     }
    ///                 ],
    ///                 \"TotalPoint\": 10.0
    ///             },
    ///             {
    ///                 \"QuestionFolderId\": "550e8400-e29b-41d4-a716-446655440001\",
    ///                 \"Criteria\": [
    ///                     {
    ///                         \"QuestionLabelId\": \"650e8400-e29b-41d4-a716-446655440001\",
    ///                         \"QuestionType\": 3,
    ///                         \"NumberOfQuestion\": 1,
    ///                         \"RawIndex\": 1
    ///                     }
    ///                 ],
    ///                 \"TotalPoint\": 5.0
    ///             }
    ///         ],
    ///         "TotalPoint": 15.0
    ///     }
    /// 
    /// </remarks>
    [HttpPost]
    [OpenApiOperation("Create a new matrix.", "")]
    public async Task<IActionResult> CreateMatrix(CreateMatrixRequest request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }


    [HttpDelete("{id}")]
    [OpenApiOperation("Delete a matrix.", "")]
    public async Task<IActionResult> DeleteMatrix(Guid id)
    {
        var request = new DeleteMatrixRequest(id);
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPut("{id}")]
    [OpenApiOperation("Update information of matrix.")]
    public async Task<IActionResult> UpdateMatrix(Guid id, UpdateMatrixRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        var result = await Mediator.Send(request);
        return Ok(result);
    }

    //write controller for GetMyMatricesRequest
    [HttpGet]
    [OpenApiOperation("Get all matrices of current user.")]
    public async Task<IActionResult> GetMyMatrices()
    {
        var request = new GetMyMatricesRequest();
        var result = await Mediator.Send(request);
        return Ok(result);
    }

}
