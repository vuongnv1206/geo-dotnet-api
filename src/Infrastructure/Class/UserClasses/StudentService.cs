using ClosedXML.Excel;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.UserStudents;
using Microsoft.AspNetCore.Http;

namespace FSH.WebApi.Infrastructure.Class.UserClasses
{
    public class StudentService : IStudentService
    {
        private readonly List<dynamic> students = new List<dynamic>
            {
                new { FirstName = "Nguyen", LastName = "Van Cao Ky", Gender = "Male", DateOfBirth = new DateTime(2000, 1, 15), Email = "kynvche163260@fpt.edu.vn", PhoneNumber = "0123456789", StudentCode = "S001" },
                new { FirstName = "Nguyen", LastName = "Van Vuong", Gender = "Female", DateOfBirth = new DateTime(1999, 5, 22), Email = "vuongnvhe163821@fpt.edu.vn", PhoneNumber = "0987654321", StudentCode = "S002" },
                new { FirstName = "Nguyen", LastName = "Minh Tam", Gender = "Female", DateOfBirth = new DateTime(2001, 8, 10), Email = "tamnmhe163882@fpt.edu.vn", PhoneNumber = "0123987654", StudentCode = "S003" },
                new { FirstName = "Nguyen", LastName = "Duc Thang", Gender = "Male", DateOfBirth = new DateTime(2002, 3, 30), Email = "thangndhe163063@fpt.edu.vn", PhoneNumber = "0345678901", StudentCode = "S004" },
                new { FirstName = "Nguyen", LastName = "Thong Duc", Gender = "Male", DateOfBirth = new DateTime(2002, 3, 30), Email = "ducnthe161707@fpt.edu.vn", PhoneNumber = "0345678901", StudentCode = "S005" }
            };
        public byte[] GenerateFormatImportStudent()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Import student");

            worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(1, 1).Value = "First name";
            worksheet.Cell(1, 2).Value = "Last name";
            worksheet.Cell(1, 3).Value = "Gender";
            worksheet.Cell(1, 4).Value = "Date of Birth";
            worksheet.Cell(1, 5).Value = "Email";
            worksheet.Cell(1, 6).Value = "Phone Number";
            worksheet.Cell(1, 7).Value = "Student Code";

            // Set column widths
            worksheet.Column(1).Width = 30; // First name
            worksheet.Column(2).Width = 30; // Last name
            worksheet.Column(3).Width = 10; // Gender
            worksheet.Column(4).Width = 20; // Date of Birth
            worksheet.Column(5).Width = 20; // Email
            worksheet.Column(6).Width = 10; // Phone Number
            worksheet.Column(7).Width = 15; // Student Code

            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Fill.BackgroundColor = XLColor.BabyBlue;

            // Place the dropdown values in cells
            worksheet.Cell("Z1").Value = "Male";
            worksheet.Cell("Z2").Value = "Female";

            // Add data validation for Gender column (dropdown)
            var genderRange = worksheet.Range("C2:C100"); // Apply dropdown from row 2 to row 100 in column C
            var dataValidation = genderRange.CreateDataValidation();
            dataValidation.List(worksheet.Range("Z1:Z2"));

            var dateRange = worksheet.Range("D2:D100");
            dateRange.Style.DateFormat.Format = "dd/mm/yyyy";

            // Add sample data to worksheet using a loop
            for (int i = 0; i < students.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = students[i].FirstName;
                worksheet.Cell(i + 2, 2).Value = students[i].LastName;
                worksheet.Cell(i + 2, 3).Value = students[i].Gender;
                worksheet.Cell(i + 2, 4).Value = students[i].DateOfBirth;
                worksheet.Cell(i + 2, 5).Value = students[i].Email;
                worksheet.Cell(i + 2, 6).Value = students[i].PhoneNumber;
                worksheet.Cell(i + 2, 7).Value = students[i].StudentCode;
            }


            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }

        public byte[] GenerateImportStudentFailed(List<FailedStudentRequest> listFail)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Import student");

            worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell(1, 1).Value = "First name";
            worksheet.Cell(1, 2).Value = "Last name";
            worksheet.Cell(1, 3).Value = "Gender";
            worksheet.Cell(1, 4).Value = "Date of Birth";
            worksheet.Cell(1, 5).Value = "Email";
            worksheet.Cell(1, 6).Value = "Phone Number";
            worksheet.Cell(1, 7).Value = "Student Code";
            worksheet.Cell(1, 8).Value = "Error";

            // Set column widths
            worksheet.Column(1).Width = 30; // First name
            worksheet.Column(2).Width = 30; // Last name
            worksheet.Column(3).Width = 10; // Gender
            worksheet.Column(4).Width = 20; // Date of Birth
            worksheet.Column(5).Width = 20; // Email
            worksheet.Column(6).Width = 10; // Phone Number
            worksheet.Column(7).Width = 15; // Student Code
            worksheet.Column(7).Width = 30; // error

            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Fill.BackgroundColor = XLColor.BabyBlue;

            // Đặt màu nền của ô thứ 8 trên dòng đầu tiên thành màu đỏ
            var cell8 = worksheet.Cell(1, 8);
            cell8.Style.Fill.BackgroundColor = XLColor.Red;

            // Place the dropdown values in cells
            worksheet.Cell("Z1").Value = "Male";
            worksheet.Cell("Z2").Value = "Female";

            // Add data validation for Gender column (dropdown)
            var genderRange = worksheet.Range("C2:C100"); // Apply dropdown from row 2 to row 100 in column C
            var dataValidation = genderRange.CreateDataValidation();
            dataValidation.List(worksheet.Range("Z1:Z2"));

            var dateRange = worksheet.Range("D2:D100");
            dateRange.Style.DateFormat.Format = "dd/mm/yyyy";

            // Add sample data to worksheet using a loop
            for (int i = 0; i < listFail.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = listFail[i].StudentRequest.FirstName;
                worksheet.Cell(i + 2, 2).Value = listFail[i].StudentRequest.LastName;
                worksheet.Cell(i + 2, 3).Value = listFail[i].StudentRequest.Gender.Value ? "Male" : "Female";
                worksheet.Cell(i + 2, 4).Value = listFail[i].StudentRequest.DateOfBirth;
                worksheet.Cell(i + 2, 5).Value = listFail[i].StudentRequest.Email;
                worksheet.Cell(i + 2, 6).Value = listFail[i].StudentRequest.PhoneNumber;
                worksheet.Cell(i + 2, 7).Value = listFail[i].StudentRequest.StudentCode;
                worksheet.Cell(i + 2, 8).Value = listFail[i].ErrorMessage;
            }


            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }

        public async Task<List<CreateStudentDto>> GetImportStudents(IFormFile file, Guid classId)
        {
            if (file == null || file.Length == 0)
            {
                return new List<CreateStudentDto>();
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            return worksheet.RowsUsed().Skip(1) // Bỏ qua hàng tiêu đề
                .Select(row => ParseStudent(row, classId))
                .Where(student => student != null)
                .ToList();
        }

        private CreateStudentDto ParseStudent(IXLRow row, Guid classId)
        {
            if (IsRowEmpty(row))
            {
                return null;
            }

            return new CreateStudentDto
            {
                FirstName = row.Cell(1).GetValue<string>(),
                LastName = row.Cell(2).GetValue<string>(),
                Gender = ParseGender(row.Cell(3).GetValue<string>()),
                DateOfBirth = ParseDateOfBirth(row.Cell(4).GetValue<string>()),
                Email = row.Cell(5).GetValue<string>(),
                PhoneNumber = row.Cell(6).GetValue<string>(),
                StudentCode = row.Cell(7).GetValue<string>(),
                ClassesId = classId
            };
        }

        private bool IsRowEmpty(IXLRow row)
        {
            return row.CellsUsed().All(cell => string.IsNullOrWhiteSpace(cell.GetValue<string>()));
        }

        private bool ParseGender(string gender)
        {
            return string.Equals(gender, "Male", StringComparison.OrdinalIgnoreCase);
        }

        private DateTime? ParseDateOfBirth(string dateOfBirth)
        {
            if (DateTime.TryParse(dateOfBirth, out var dob))
            {
                return dob;
            }
            return null;
        }

    }

}