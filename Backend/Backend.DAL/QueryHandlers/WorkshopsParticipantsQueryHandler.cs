using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.QueryHandlers
{
  public class WorkshopsParticipantsQueryHandler : GenericQueryHandler<DTOs.WorkshopParticipant, EF.WorkshopParticipant>
  {
    
    public WorkshopsParticipantsQueryHandler(EF.FrontedContext ctx, ILogger<WorkshopsParticipantsQueryHandler> logger) : base(ctx, logger)
    {
      
    }

    protected override Expression<Func<EF.WorkshopParticipant, DTOs.WorkshopParticipant>> Selector =>
      t => new DTOs.WorkshopParticipant
      {
        Id = t.Id,
        StudentId = t.ParticipantId,
        Name = t.Participant.Name,
        Surname = t.Participant.Surname,        
        Email = t.Participant.Email,
        WorkshopId = t.WorkshopId,
        Workshop = t.Workshop.Title,
        WorkshopSchool = $"{t.Workshop.School.Name} {t.Workshop.School.Town.Name}",
        StudentSchool = $"{t.Participant.School.Name} {t.Participant.School.Town.Name}"
      };

    protected override Dictionary<string, Expression<Func<EF.WorkshopParticipant, object>>> OrderSelectors => orderSelectors;

    private static Dictionary<string, Expression<Func<EF.WorkshopParticipant, object>>> orderSelectors = new Dictionary<string, Expression<Func<EF.WorkshopParticipant, object>>>
    {
      [nameof(DTOs.WorkshopParticipant.Id).ToUpper()] = a => a.Id,
      [nameof(DTOs.WorkshopParticipant.Name).ToUpper()] = a => a.Participant.Name,
      [nameof(DTOs.WorkshopParticipant.Surname).ToUpper()] = a => a.Participant.Surname,
      [nameof(DTOs.WorkshopParticipant.Email).ToUpper()] = a => a.Participant.Email,
      [nameof(DTOs.WorkshopParticipant.WorkshopId).ToUpper()] = a => a.WorkshopId,
      [nameof(DTOs.WorkshopParticipant.Workshop).ToUpper()] = a => a.Workshop.Title,
      [nameof(DTOs.WorkshopParticipant.StudentId).ToUpper()] = a => a.ParticipantId,
      [nameof(DTOs.WorkshopParticipant.WorkshopSchool).ToUpper()] = a => $"{a.Workshop.School.Name} {a.Workshop.School.Town.Name}",
      [nameof(DTOs.WorkshopParticipant.StudentSchool).ToUpper()] = a => $"{a.Participant.School.Name} {a.Participant.School.Town.Name}"
    };

    protected override Expression<Func<EF.WorkshopParticipant, bool>> CreateWherePredicate(string field, string @operator, string value)
    {
#warning ignoring @operator - (used just to demonstrate overriding)
      if (field.Equals(nameof(DTOs.WorkshopParticipant.WorkshopSchool), StringComparison.OrdinalIgnoreCase))
      {
        return a => (a.Workshop.School.Name + " " + a.Workshop.School.Town.Name).Contains(value);
      }
      else if (field.Equals(nameof(DTOs.WorkshopParticipant.StudentSchool), StringComparison.OrdinalIgnoreCase))
      {
        return a => (a.Participant.School.Name + " " + a.Participant.School.Town.Name).Contains(value);
      }
      else
      {
        return base.CreateWherePredicate(field, @operator, value);
      }

    }
  }
}
