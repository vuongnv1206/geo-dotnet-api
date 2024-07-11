using FSH.WebApi.Application.Class.Dto;

namespace FSH.WebApi.Application.Class.SharedClasses;
public class SharedGroupClassDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Name { get; set; }
    public List<SharedClassDto>? Classes { get; set; } = new();
}
