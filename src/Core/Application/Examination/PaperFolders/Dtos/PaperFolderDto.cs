﻿

namespace FSH.WebApi.Application.Examination.PaperFolders;
public class PaperFolderDto : IDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? SubjectId { get; set; }
}
