
using FSH.WebApi.Application.Examination.Matrices;
using FSH.WebApi.Domain.Question.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace FSH.WebApi.In.Examination.Matrices.Helpers;
public class ContentFormatAttribute : ValidationAttribute
{

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Content is required.");
        }

        var content = value as string;
        if (content == null)
        {
            return new ValidationResult("Content must be a string.");
        }

        List<ContentMatrixDto> contentItems;
        try
        {
            contentItems = JsonConvert.DeserializeObject<List<ContentMatrixDto>>(content);
        }
        catch (Exception)
        {
            return new ValidationResult("Content is not a valid JSON array.");
        }


        var allRawIndexes = new HashSet<int>();

        foreach (var item in contentItems)
        {

            if (item.TotalPoint <= 0)
            {
                return new ValidationResult("TotalPoint for each ContentMatrixDto must be greater than zero.");
            }


            if (item.CriteriaQuestions == null || !item.CriteriaQuestions.Any())
            {
                return new ValidationResult("CriteriaQuestions cannot be null or empty.");
            }

            foreach (var criteria in item.CriteriaQuestions)
            {
                if (!Enum.IsDefined(typeof(QuestionType), criteria.QuestionType))
                {
                    return new ValidationResult($"QuestionType '{criteria.QuestionType}' is not valid.");
                }
            }

        }

        return ValidationResult.Success;
    }
}