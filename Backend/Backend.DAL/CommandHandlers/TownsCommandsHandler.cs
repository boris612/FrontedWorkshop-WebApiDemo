using AutoMapper;
using Backend.DAL.Models;
using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.CommandHandlers
{
  public class TownsCommandsHandler : GenericCommandHandler<EF.Town, DTOs.Town>                      
  {
    public TownsCommandsHandler(FrontedContext ctx, ILogger<TownsCommandsHandler> logger, IMapper mapper) : base(ctx, logger, mapper)
    {
    }
  }
}
