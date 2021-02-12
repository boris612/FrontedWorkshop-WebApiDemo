using AutoMapper;
using Backend.DAL.Models;
using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.CommandHandlers
{
  public class WorkshopsCommandsHandler : GenericCommandHandler<EF.Workshop, DTOs.Workshop>                      
  {
    public WorkshopsCommandsHandler(FrontedContext ctx, ILogger<WorkshopsCommandsHandler> logger, IMapper mapper) : base(ctx, logger, mapper)
    {
    }
  }
}
