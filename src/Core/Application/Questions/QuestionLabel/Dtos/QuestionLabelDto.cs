using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Questions.QuestionLabel;
public class QuestionLabelDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}
