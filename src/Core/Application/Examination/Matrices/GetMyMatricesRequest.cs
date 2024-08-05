﻿using FSH.WebApi.Domain.Examination;


namespace FSH.WebApi.Application.Examination.Matrices;
public class GetMyMatricesRequest : IRequest<List<PaperMatrixDto>>
{
}

public class GetMyMatricesRequestHandler : IRequestHandler<GetMyMatricesRequest, List<PaperMatrixDto>>
{
    private readonly IReadRepository<PaperMatrix> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly ISerializerService _serializerService;
    public GetMyMatricesRequestHandler(IReadRepository<PaperMatrix> repository, ICurrentUser currentUser, ISerializerService serializerService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _serializerService = serializerService;
    }

    public async Task<List<PaperMatrixDto>> Handle(GetMyMatricesRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetUserId();
        var matrices = await _repository.ListAsync(new MyPaperMatrixSpec(currentUserId), cancellationToken);

        var matrixDtos = matrices.Select(matrix => new PaperMatrixDto
        {
            Id = matrix.Id,
            Name = matrix.Name,
            Content = matrix.Content,
            // Deserialize Content từ JSON string sang List<ContentMatrixDto>
            ContentItems = _serializerService.Deserialize<List<ContentMatrixDto>>(matrix.Content),
            TotalPoint = matrix.TotalPoint
        }).ToList();

        return matrixDtos;
    }
}
