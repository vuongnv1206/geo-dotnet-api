using FSH.WebApi.Application.Class.Dto;
using FSH.WebApi.Application.Class.New.Dto;
using FSH.WebApi.Application.Class.New.Spec;
using FSH.WebApi.Application.Class.UserClasses;
using FSH.WebApi.Domain.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Class.New;
public class GetNewsRequest : IRequest<List<NewsDto>>
{
    public Guid? ClassesId { get; set; }
    public class GetNewsRequestHandler : IRequestHandler<GetNewsRequest, List<NewsDto>>
    {
        private readonly IReadRepository<News> _repository;
        private readonly INewReactionRepository _newReactionRepository;

        public GetNewsRequestHandler(IReadRepository<News> repository, INewReactionRepository newReactionRepository)
        {
            _repository = repository;
            _newReactionRepository = newReactionRepository;
        }

        public async Task<List<NewsDto>> Handle(GetNewsRequest request, CancellationToken cancellationToken)
        {
            List<NewsDto> news;
            var spec = new NewsBySearchRequestWithClass(request.ClassesId);

            news = await _repository.ListAsync(spec, cancellationToken);

            foreach (var newDto in news)
            {
                var newCount = await _newReactionRepository.CountLikeOfUserInNews(newDto.Id);
                newDto.NumberLikeInTheNews = newCount;
            }
            return news;
        }
    }
}

