using ElectricityDataAggregator.Application.AggregateDatas.Query;
using ElectricityDataAggregator.Common.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectricityDataAggregator.API.Controllers
{
    public class MainController : ApiControllerBase
    {
        public MainController(IMediator mediator)
            : base(mediator)
        { }

        [HttpGet]
        [SwaggerOperation("Get aggregated data")]
        public async Task<GetAggregatedDataQueryResponse> GetAggregatedData()
            => await mediator.Send(new GetAggregatedDataQuery());
    }
}
