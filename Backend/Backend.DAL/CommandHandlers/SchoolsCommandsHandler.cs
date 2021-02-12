using AutoMapper;
using Backend.DAL.Models;
using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.CommandHandlers
{
  public class SchoolsCommandsHandler : GenericCommandHandler<EF.School, DTOs.School>                      
  {
    public SchoolsCommandsHandler(FrontedContext ctx, ILogger<SchoolsCommandsHandler> logger, IMapper mapper) : base(ctx, logger, mapper)
    {
    }
  }
}
