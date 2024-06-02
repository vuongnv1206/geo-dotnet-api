using FSH.WebApi.Application.Catalog.Products;
using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Common.Persistence;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class;
public class SearchClassesRequest : PaginationFilter, IRequest<PaginationResponse<ClassDto>>
{
    public Guid? GroupClassId { get; set; }

    public class SearchClassesRequestHandler : IRequestHandler<SearchClassesRequest, PaginationResponse<ClassDto>>
    {
        private readonly IReadRepository<Classes> _repository;
        private readonly ICurrentUser _currentUser;
        public SearchClassesRequestHandler(IReadRepository<Classes> repository, ICurrentUser currentUser)
        {
            _currentUser = currentUser;
            _repository = repository;
        }

        public async Task<PaginationResponse<ClassDto>> Handle(SearchClassesRequest request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated())
            {
                throw new Exception("User not authenticated");
            }

            var userId = _currentUser.GetUserId();
            var spec = new ClassesBySearchRequestWithGroupClassSpec(request, userId);
            return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
        }
    }
}
