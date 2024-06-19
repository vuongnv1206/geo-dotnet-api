using Microsoft.AspNetCore.Http;
using System.Text;
using Xceed.Words.NET;

namespace FSH.WebApi.Host.Controllers.Question;

public class ReadQuestionsFromFileRequest : IRequest<string[]>
{
    [AllowedExtensions(".docx", ".txt")] // Only .docx files and .txt files are allowed
    [MaxFileSize(20 * 1024 * 1024)] // 20 MB
    public required IFormFile[] Files { get; set; }
}

public class ReadQuestionsFromFileRequestHandler : IRequestHandler<ReadQuestionsFromFileRequest, string[]>
{
    private readonly IFileStorageService _fileStorageService;

    public ReadQuestionsFromFileRequestHandler(IFileStorageService fileStorageService) => _fileStorageService = fileStorageService;

    public async Task<string[]> Handle(ReadQuestionsFromFileRequest request, CancellationToken cancellationToken)
    {
        // Only .docx files and .txt files are allowed
        string[] allowedExtensions = new[] { ".docx", ".txt" };
        string allowedExtensionsMessage = string.Join(", ", allowedExtensions);

        foreach (var file in request.Files)
        {
            string extension = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extension))
            {
                throw new BadRequestException($"Only {allowedExtensionsMessage} files are allowed.");
            }
        }

        string[] listFilePaths = await _fileStorageService.SaveFilesAsync(request.Files, cancellationToken);

        List<string> questions = new();
        foreach (string filePath in listFilePaths)
        {
            if (Path.GetExtension(filePath) == ".docx")
            {
                // Read questions from .docx file
                using DocX doc = DocX.Load(filePath);
                StringBuilder sb = new StringBuilder();
                foreach (var paragraph in doc.Paragraphs)
                {
                    sb.AppendLine(paragraph.Text);
                }

                questions.Add(sb.ToString().Replace("\r\n", "\n"));
            }
            else if (Path.GetExtension(filePath) == ".txt")
            {
                // Read questions from .txt file
                questions.Add(File.ReadAllText(filePath).Replace("\r\n", "\n"));
            }
        }

        return questions.ToArray();
    }
}