using AutoMapper;
using Backend.DAL.Models;
using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.CommandHandlers
{
  public class WorkshopsParticipantsCommandsHandler : GenericCommandHandler<EF.WorkshopParticipant, DTOs.WorkshopParticipant>                      
  {
    public WorkshopsParticipantsCommandsHandler(FrontedContext ctx, ILogger<WorkshopsCommandsHandler> logger, IMapper mapper) : base(ctx, logger, mapper)
    {
    }
  }
}
