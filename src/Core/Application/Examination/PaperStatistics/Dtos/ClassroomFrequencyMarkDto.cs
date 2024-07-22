using FSH.WebApi.Application.Class.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Examination.PaperStatistics;
public class ClassroomFrequencyMarkDto
{
    public ClassDto Class { get; set; }
    public List<StudentDto> Students { get; set; }
    public int TotalRegister { get; set; }
    public int TotalAttendee { get; set; }
    public List<FrequencyMarkDto> FrequencyMarks { get; set; }
}
