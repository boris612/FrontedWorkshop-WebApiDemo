using Backend.Core;
using Backend.Core.Queries.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.DAL_EF.Core
{
  public abstract class GenericQueryHandler<TDto, TDal> : IRequestHandler<GetItemsQuery<TDto>, IEnumerable<TDto>>,
                                                          IRequestHandler<GetSingleItemQuery<TDto>, TDto>,
                                                          IRequestHandler<GetCountQuery<TDto>, int>
                                                          where TDal : class, IHasIntegerId
  {    
    private readonly DbContext ctx;
    private readonly ILogger logger;

    public GenericQueryHandler(DbContext ctx, ILogger logger)
    {
      this.ctx = ctx;
      this.logger = logger;
    }

    protected abstract Expression<Func<TDal, TDto>> Selector { get; }    
    protected abstract Dictionary<string, Expression<Func<TDal, object>>> OrderSelectors { get; }
    protected virtual Dictionary<string, Expression<Func<TDal, object>>> WherePredicates => OrderSelectors;
    protected virtual Expression<Func<TDal, bool>> PKWherePredicate(int value)
    {
      return dal => dal.Id == value;
    }

    protected virtual Expression<Func<TDal, bool>> CreateWherePredicate(string field, string @operator, string value)
    {
      if (WherePredicates.TryGetValue(field.ToUpper(), out var expression))
      {
        var predicate = expression.BuildWherePredicate(value, @operator, logger);
        return predicate;
      }
      else
      {
        logger.LogWarning($"Unknown filter field: {field} with value {value} in {this.GetType().Name}");
        return null;
      }      
    }

    public virtual async Task<IEnumerable<TDto>> Handle(GetItemsQuery<TDto> request, CancellationToken cancellationToken)
    {
      var query = ctx.Set<TDal>().AsNoTracking();

      query = query.ApplyFilterData(request.Filters, CreateWherePredicate);
      query = query.ApplySort(request.Sort, OrderSelectors);

      if (request.Paging != null && request.Paging.Count > 0)
      {
        query = query.Skip(request.Paging.From)
                     .Take(request.Paging.Count);
      }         
            
      var data = await query.Select(Selector)
                            .ToListAsync(cancellationToken);
      return data;
    }

    public virtual async Task<int> Handle(GetCountQuery<TDto> request, CancellationToken cancellationToken)
    {
      var query = ctx.Set<TDal>().AsNoTracking();
      query = query.ApplyFilterData(request.Filters, CreateWherePredicate);
      int count = await query.CountAsync(cancellationToken);
      return count;
    }

    public virtual async Task<TDto> Handle(GetSingleItemQuery<TDto> request, CancellationToken cancellationToken)
    {
      var item = await ctx.Set<TDal>()                      
                          .Where(PKWherePredicate(request.Id))
                          .Select(Selector)
                          .FirstOrDefaultAsync();
      return item;
    }    
  }
}
