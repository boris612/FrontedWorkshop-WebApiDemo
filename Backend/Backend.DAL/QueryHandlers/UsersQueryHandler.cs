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
  public class UsersQueryHandler : GenericQueryHandler<DTOs.User, EF.User>
  {
    
    public UsersQueryHandler(EF.FrontedContext ctx, ILogger<UsersQueryHandler> logger) : base(ctx, logger)
    {
      
    }

    protected override Expression<Func<User, bool>> PKWherePredicate(int value)
    {
      return u => u.UserId == value;
    }

    protected override Expression<Func<EF.User, DTOs.User>> Selector =>
      t => new DTOs.User
      {
        Id = t.UserId,
        FirstName = t.FirstName,
        LastName = t.LastName,
        UserName = t.Name
      };

    protected override Dictionary<string, Expression<Func<EF.User, object>>> OrderSelectors => orderSelectors;

    private static Dictionary<string, Expression<Func<EF.User, object>>> orderSelectors = new Dictionary<string, Expression<Func<EF.User, object>>>
    {
      [nameof(DTOs.User.Id).ToUpper()] = a => a.UserId,
      [nameof(DTOs.User.FirstName).ToUpper()] = a => a.FirstName,
      [nameof(DTOs.User.LastName).ToUpper()] = a => a.LastName,
      [nameof(DTOs.User.UserName).ToUpper()] = a => a.Name
    };    
  }
}
