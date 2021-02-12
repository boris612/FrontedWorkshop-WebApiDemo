using Backend.DAL.Models;
using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.QueryHandlers
{
  public class WorkshopsQueryHandler : GenericQueryHandler<DTOs.Workshop, EF.ViewWorkshops>
  {
    
    public WorkshopsQueryHandler(EF.FrontedContext ctx, ILogger<WorkshopsQueryHandler> logger) : base(ctx, logger)
    {
      
    }

    protected override Expression<Func<EF.ViewWorkshops, DTOs.Workshop>> Selector =>
      t => new DTOs.Workshop
      {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Time = t.Time,
        SchoolId = t.SchoolId,
        School = t.School,
        Capacity = t.Capacity,
        NoOfParticipants = t.NoOfParticipants,
        FreePlaces = t.FreePlaces
      };

    protected override Dictionary<string, Expression<Func<EF.ViewWorkshops, object>>> OrderSelectors => orderSelectors;

    private static Dictionary<string, Expression<Func<EF.ViewWorkshops, object>>> orderSelectors = new Dictionary<string, Expression<Func<EF.ViewWorkshops, object>>>
    {
      [nameof(DTOs.Workshop.Id).ToUpper()] = a => a.Id,
      [nameof(DTOs.Workshop.Title).ToUpper()] = a => a.Title,
      [nameof(DTOs.Workshop.Time).ToUpper()] = a => a.Time,
      [nameof(DTOs.Workshop.Description).ToUpper()] = a => a.Description,
      [nameof(DTOs.Workshop.SchoolId).ToUpper()] = a => a.SchoolId,
      [nameof(DTOs.Workshop.School).ToUpper()] = a => a.School,
      [nameof(DTOs.Workshop.Capacity).ToUpper()] = a => a.Capacity,
      [nameof(DTOs.Workshop.NoOfParticipants).ToUpper()] = a => a.NoOfParticipants,
      [nameof(DTOs.Workshop.FreePlaces).ToUpper()] = a => a.FreePlaces
    };
  }
}
