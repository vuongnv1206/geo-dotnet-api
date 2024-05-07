﻿using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class GetClassesRequest : IRequest<ClassDto>
{
    public Guid Id { get; set; }

    public GetClassesRequest(Guid id) => Id = id;
}

public class GetClassRequestHandler : IRequestHandler<GetClassesRequest, ClassDto>
{
    private readonly IRepository<Classes> _repository;
    private readonly IStringLocalizer _t;

    public GetClassRequestHandler(IRepository<Classes> repository, IStringLocalizer<GetClassRequestHandler> localizer) =>
        (_repository, _t) = (repository, localizer);
    public async Task<ClassDto> Handle(GetClassesRequest request, CancellationToken cancellationToken)
    {
        return await _repository.FirstOrDefaultAsync(
            (ISpecification<Classes, ClassDto>)new ClassByIdWithGroupClass(request.Id), cancellationToken)
        ?? throw new NotFoundException(_t["Classes {0} Not Found.", request.Id]);
    }
}
