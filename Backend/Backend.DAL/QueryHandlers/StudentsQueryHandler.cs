using AutoMapper;
using Backend.DAL_EF.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.QueryHandlers
{
  public class StudentsQueryHandler : GenericQueryHandler<DTOs.Student, EF.Student>
  {
    
    public StudentsQueryHandler(EF.FrontedContext ctx, ILogger<StudentsQueryHandler> logger) : base(ctx, logger)
    {
      
    }

    protected override Expression<Func<EF.Student, DTOs.Student>> Selector =>
      t => new DTOs.Student
      {
        Id = t.Id,
        Name = t.Name,
        Surname = t.Surname,
        Email = t.Email,
        SchoolId = t.SchoolId,
        School = t.School.Name,
        Town = $"{ t.School.Town.Postcode } { t.School.Town.Name }"
      };

    protected override Dictionary<string, Expression<Func<EF.Student, object>>> OrderSelectors => orderSelectors;

    private static Dictionary<string, Expression<Func<EF.Student, object>>> orderSelectors = new Dictionary<string, Expression<Func<EF.Student, object>>>
    {
      [nameof(DTOs.Student.Id).ToUpper()] = a => a.Id,
      [nameof(DTOs.Student.Name).ToUpper()] = a => a.Name,
      [nameof(DTOs.Student.Surname).ToUpper()] = a => a.Surname,
      [nameof(DTOs.Student.Email).ToUpper()] = a => a.Email,
      [nameof(DTOs.Student.School).ToUpper()] = a => a.School.Name,
      [nameof(DTOs.Student.Town).ToUpper()] = a => a.School.Town.Name
    };    
  }
}
