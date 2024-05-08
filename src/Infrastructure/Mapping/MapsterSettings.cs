using FSH.WebApi.Application.Questions.Dtos;
using FSH.WebApi.Domain.Question;
using Mapster;

namespace FSH.WebApi.Infrastructure.Mapping;

public class MapsterSettings
{
    public static void Configure()
    {
        // here we will define the type conversion / Custom-mapping
        // More details at https://github.com/MapsterMapper/Mapster/wiki/Custom-mapping

        // This one is actually not necessary as it's mapped by convention
        // TypeAdapterConfig<Product, ProductDto>.NewConfig().Map(dest => dest.BrandName, src => src.Brand.Name);

        // Map QuestionFolder to QuestionTreeDto
        TypeAdapterConfig<QuestionFolder, QuestionTreeDto>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.Permission, src => src.Permissions)
            .Map(dest => dest.Children, src => src.Children);
    }
}