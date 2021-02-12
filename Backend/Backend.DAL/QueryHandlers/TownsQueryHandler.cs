using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.QueryHandlers
{
  public class TownsQueryHandler : GenericQueryHandler<DTOs.Town, EF.Town>
  {
    
    public TownsQueryHandler(EF.FrontedContext ctx, ILogger<TownsQueryHandler> logger) : base(ctx, logger)
    {
      
    }

    protected override Expression<Func<EF.Town, DTOs.Town>> Selector =>
      t => new DTOs.Town
      {
        Id = t.Id,
        Name = t.Name,
        Postcode = t.Postcode
      };

    protected override Dictionary<string, Expression<Func<EF.Town, object>>> OrderSelectors => orderSelectors;

    private static Dictionary<string, Expression<Func<EF.Town, object>>> orderSelectors = new Dictionary<string, Expression<Func<EF.Town, object>>>
    {
      [nameof(DTOs.Town.Id).ToUpper()] = a => a.Id,
      [nameof(DTOs.Town.Name).ToUpper()] = a => a.Name,
      [nameof(DTOs.Town.Postcode).ToUpper()] = a => a.Postcode
    };    
  }
}
