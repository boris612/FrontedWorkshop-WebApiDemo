using AutoMapper;
using Backend.Core;
using Backend.Core.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.DAL_EF.Core
{
  public class GenericCommandHandler<TDal, TDto> : IRequestHandler<AddCommand<TDto>, int>,
                                            IRequestHandler<UpdateCommand<TDto>>,
                                            IRequestHandler<DeleteCommand<TDto>>
                                            where TDal:class, IHasIntegerId
                                            where TDto:IHasIntegerId
  {
    private readonly DbContext ctx;
    private readonly ILogger logger;
    private readonly IMapper mapper;

    protected GenericCommandHandler(DbContext ctx, ILogger logger, IMapper mapper)
    {
      this.ctx = ctx;
      this.logger = logger;
      this.mapper = mapper;
    }

    public virtual async Task<int> Handle(AddCommand<TDto> request, CancellationToken cancellationToken)
    {
      var entity = mapper.Map<TDto, TDal>(request.Dto);
      ctx.Add(entity);
      await ctx.SaveChangesAsync();
      return entity.Id;
    }

    public virtual async Task<Unit> Handle(UpdateCommand<TDto> request, CancellationToken cancellationToken)
    {
      var entity = await ctx.Set<TDal>().FindAsync(request.Dto.Id);
      if (entity != null)
      {
        mapper.Map(request.Dto, entity);        
        await ctx.SaveChangesAsync();
        return default;
      }
      else
      {
        logger.LogError($"UpdateCommand<{typeof(TDto).Name}> : Invalid id #{request.Dto.Id}");
        throw new ArgumentException($"Invalid id: {request.Dto.Id}");
      }
    }

    public virtual async Task<Unit> Handle(DeleteCommand<TDto> request, CancellationToken cancellationToken)
    {
      var item = await ctx.Set<TDal>().FindAsync(request.Id);
      if (item != null)
      {
        ctx.Remove(item);
        await ctx.SaveChangesAsync();
        return default;
      }
      else
      {
        logger.LogError($"DeleteCommand<{typeof(TDto).Name}> : Invalid id #{request.Id}");
        throw new ArgumentException($"Invalid id: {request.Id}");
      }
    }    
  }
}
