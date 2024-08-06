using FSH.WebApi.Domain.Question;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Domain.Examination;
public class PaperMatrix : AuditableEntity, IAggregateRoot
{
    public string Name { get; set; }
    // Format : [{QuestionFolderId},{Criteria:[{QuestionLabelId},{QuestionType},{NumberOfQuestion},{RawIndex}]},{TotalPoint}]
    public string Content { get; set; }
    public float TotalPoint { get; set; }

    public PaperMatrix()
    {
    }

    public PaperMatrix(string name, string content, float totalPoint)
    {
        Name = name;
        Content = content;
        TotalPoint = totalPoint;
    }

    public void Update(string name, string content, float totalPoint)
    {
        Name = name;
        Content = content;
        TotalPoint = totalPoint;
    }

}




