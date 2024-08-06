
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
            // Remove CriteriaQuestion where NumberOfQuestion is 0
            item.CriteriaQuestions = item.CriteriaQuestions.Where(c => c.NumberOfQuestion > 0).ToList();

            if (item.TotalPoint <= 0)
            {
                return new ValidationResult("TotalPoint for each ContentMatrixDto must be greater than zero.");
            }


            if (item.CriteriaQuestions == null || !item.CriteriaQuestions.Any())
            {
                return new ValidationResult("CriteriaQuestions cannot be null or empty.");
            }

            // If there are no valid CriteriaQuestions, remove the entire item
            //if (!item.CriteriaQuestions.Any())
            //{
            //    contentItems.Remove(item);
            //    continue;
            //}


            foreach (var criteria in item.CriteriaQuestions)
            {
                if (!Enum.IsDefined(typeof(QuestionType), criteria.QuestionType))
                {
                    return new ValidationResult($"QuestionType '{criteria.QuestionType}' is not valid.");
                }

                if (string.IsNullOrWhiteSpace(criteria.RawIndex))
                {
                    // Tìm giá trị trống trong khoảng từ 1 đến giá trị lớn nhất hiện có
                    var maxIndex = allRawIndexes.Any() ? allRawIndexes.Max() : 0;
                    var missingIndexes = Enumerable.Range(1, maxIndex).Except(allRawIndexes).ToList();

                    // Lấy đủ số lượng cần thiết từ các giá trị trống hoặc tạo mới
                    var newIndexes = missingIndexes.Take(criteria.NumberOfQuestion).ToList();
                    if (newIndexes.Count < criteria.NumberOfQuestion)
                    {
                        newIndexes.AddRange(Enumerable.Range(maxIndex + 1, criteria.NumberOfQuestion - newIndexes.Count));
                    }

                    criteria.RawIndex = string.Join(",", newIndexes);
                }

                var indexes = criteria.RawIndex.Split(',').Select(int.Parse).ToList();
                if (indexes.Count != criteria.NumberOfQuestion)
                {
                    return new ValidationResult($"The number of RawIndex values '{criteria.RawIndex}' does not match the NumberOfQuestion '{criteria.NumberOfQuestion}'.");
                }

                foreach (var index in indexes)
                {
                    if (!allRawIndexes.Add(index))
                    {
                        return new ValidationResult($"RawIndex '{index}' is duplicated across CriteriaQuestions.");
                    }
                }
            }

        }

        return ValidationResult.Success;
    }
}