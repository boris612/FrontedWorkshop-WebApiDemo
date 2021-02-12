using AutoMapper;
using Backend.DAL.Models;
using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.CommandHandlers
{
  public class UsersCommandsHandler : GenericCommandHandler<EF.User, DTOs.User>                      
  {
    public UsersCommandsHandler(FrontedContext ctx, ILogger<UsersCommandsHandler> logger, IMapper mapper) : base(ctx, logger, mapper)
    {
    }
  }
}
