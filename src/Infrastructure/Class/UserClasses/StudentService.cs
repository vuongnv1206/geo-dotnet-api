﻿using ClosedXML.Excel;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.UserStudents;
using Microsoft.AspNetCore.Http;

namespace FSH.WebApi.Infrastructure.Class.UserClasses
{
    public class StudentService : IStudentService
    {
        private readonly List<dynamic> students = new List<dynamic>
            {
                new { FirstName = "John", LastName = "Doe", Gender = "Male", DateOfBirth = new DateTime(2000, 1, 15), Email = "johndoe@example.com", PhoneNumber = "0123456789", StudentCode = "S001" },
                new { FirstName = "Jane", LastName = "Smith", Gender = "Female", DateOfBirth = new DateTime(1999, 5, 22), Email = "janesmith@example.com", PhoneNumber = "0987654321", StudentCode = "S002" },
                new { FirstName = "Alice", LastName = "Johnson", Gender = "Female", DateOfBirth = new DateTime(2001, 8, 10), Email = "alicejohnson@example.com", PhoneNumber = "0123987654", StudentCode = "S003" },
                new { FirstName = "Bob", LastName = "Brown", Gender = "Male", DateOfBirth = new DateTime(2002, 3, 30), Email = "bobbrown@example.com", PhoneNumber = "0345678901", StudentCode = "S004" }
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

        public async Task<List<CreateStudentDto>> GetImportStudents(IFormFile file, Guid classId)
        {
            var students = new List<CreateStudentDto>();
            if (file == null || file.Length == 0)
            {
                return students;
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);

                    for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                    {
                        var student = new CreateStudentDto
                        {
                            FirstName = worksheet.Cell(row, 1).GetValue<string>(),
                            LastName = worksheet.Cell(row, 2).GetValue<string>(),
                            Gender = worksheet.Cell(row, 3).GetValue<string>() == "Male",
                            DateOfBirth = worksheet.Cell(row, 4).GetDateTime(),
                            Email = worksheet.Cell(row, 5).GetValue<string>(),
                            PhoneNumber = worksheet.Cell(row, 6).GetValue<string>(),
                            StudentCode = worksheet.Cell(row, 7).GetValue<string>(),
                            ClassesId = classId
                        };
                        students.Add(student);
                    }

                }
            }

            return students;
        }
    }
}
