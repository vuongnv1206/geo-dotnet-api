using FSH.WebApi.Application.Common.Exporters;
using FSH.WebApi.Domain.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.WebApi.Application.Assignments;

//public class ExportAssignmentsRequest : BaseFilter, IRequest<Stream>
//{
//    public Guid? SubjectId { get; set; }
//    public decimal? MinimumRate { get; set; }
//    public decimal? MaximumRate { get; set; }
//}

//public class ExportAssignmentsRequestHandler : IRequestHandler<ExportAssignmentsRequest, Stream>
//{
//    private readonly IReadRepository<Assignment> _repository;
//    private readonly IExcelWriter _excelWriter;

//    public ExportAssignmentsRequestHandler(IReadRepository<Assignment> repository, IExcelWriter excelWriter)
//    {
//        _repository = repository;
//        _excelWriter = excelWriter;
//    }

//    //public async Task<Stream> Handle(ExportAssignmentsRequest request, CancellationToken cancellationToken)
//    //{
//    //    var spec = new ExportAssignmentsWithSubjectsSpecification(request);

//    //    var list = await _repository.ListAsync(spec, cancellationToken);

//    //    return _excelWriter.WriteToStream(list);
//    //}
//}

////public class ExportAssignmentsWithSubjectsSpecification : EntitiesByBaseFilterSpec<Assignment, AssignmentExportDto>
////{
////    public ExportAssignmentsWithSubjectsSpecification(ExportAssignmentsRequest request)
////        : base(request) =>
////        Query
////            .Include(p => p.Subject)
////            .Where(p => p.SubjectId.Equals(request.SubjectId!.Value), request.SubjectId.HasValue)
////            .Where(p => p.Rate >= request.MinimumRate!.Value, request.MinimumRate.HasValue)
////            .Where(p => p.Rate <= request.MaximumRate!.Value, request.MaximumRate.HasValue);
////}