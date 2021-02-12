using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Backend.Contract.DTOs;
using Backend.Contract.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.WebApi.Controllers
{
  [ApiController]
  [Route("[controller]/[action]")]
  public class LookupController : ControllerBase
  {
    private readonly IMediator mediator;
    private readonly ILogger<LookupController> logger;  

    public LookupController(IMediator mediator, ILogger<LookupController> logger)
    {
      this.mediator = mediator;
      this.logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<TextValue<int>>> Towns()
    {
      var request = new LookupQuery<Town, int>();      
      var data = await mediator.Send(request);
      return data;
    }

    [HttpGet]
    public async Task<IEnumerable<TextValue<int>>> Schools()
    {
      var request = new LookupQuery<School, int>();
      var data = await mediator.Send(request);
      return data;
    }

    [HttpGet]
    public async Task<IEnumerable<TextValue<int>>> Students(int workshopId)
    {
      var request = new AvailableParticipantsQuery
      {
        WorkshopId = workshopId
      };
      var data = await mediator.Send(request);
      return data;
    }
  }
}
