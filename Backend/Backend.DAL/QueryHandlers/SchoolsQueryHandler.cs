using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.QueryHandlers
{
  public class SchoolsQueryHandler : GenericQueryHandler<DTOs.School, EF.School>
  {
    
    public SchoolsQueryHandler(EF.FrontedContext ctx, ILogger<SchoolsQueryHandler> logger) : base(ctx, logger)
    {
      
    }

    protected override Expression<Func<EF.School, DTOs.School>> Selector =>
      t => new DTOs.School
      {
        Id = t.Id,
        Name = t.Name,
        TownId = t.TownId,
        Town = $"{t.Town.Name} ({t.Town.Postcode})"
      };

    protected override Dictionary<string, Expression<Func<EF.School, object>>> OrderSelectors => orderSelectors;

    private static Dictionary<string, Expression<Func<EF.School, object>>> orderSelectors = new Dictionary<string, Expression<Func<EF.School, object>>>
    {
      [nameof(DTOs.School.Id).ToUpper()] = a => a.Id,
      [nameof(DTOs.School.Name).ToUpper()] = a => a.Name,
      [nameof(DTOs.School.TownId).ToUpper()] = a => a.TownId,
      [nameof(DTOs.School.Town).ToUpper()] = a => a.Town.Name
    };    
  }
}
