using Backend.Contract.DTOs;
using Backend.Contract.Queries;
using Backend.DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DTOs = Backend.Contract.DTOs;
using EF = Backend.DAL.Models;

namespace Backend.DAL.QueryHandlers
{
  public class LookupQueryHandler : IRequestHandler<LookupQuery<DTOs.Town, int>, IEnumerable<TextValue<int>>>,
                                    IRequestHandler<LookupQuery<DTOs.School, int>, IEnumerable<TextValue<int>>>,
                                    IRequestHandler<AvailableParticipantsQuery, IEnumerable<TextValue<int>>>
  {
    private readonly FrontedContext ctx;

    public LookupQueryHandler(FrontedContext ctx)
    {
      this.ctx = ctx;
    }

    public async Task<IEnumerable<TextValue<int>>> Handle(LookupQuery<DTOs.Town, int> request, CancellationToken cancellationToken)
    {
      var list = await ctx.Town
                          .OrderBy(a => a.Name)
                          .Select(a => new TextValue<int>
                          {
                            Value = a.Id,
                            Text = $"{a.Name} ({a.Postcode})"
                          })
                          .ToListAsync(cancellationToken);
      return list;
    }

    public async Task<IEnumerable<TextValue<int>>> Handle(LookupQuery<DTOs.School, int> request, CancellationToken cancellationToken)
    {
      var list = await ctx.School
                          .OrderBy(a => a.Name)
                          .Select(a => new TextValue<int>
                          {
                            Value = a.Id,
                            Text = $"{a.Name} ({a.Town.Postcode} {a.Town.Name})"
                          })
                          .ToListAsync(cancellationToken);
      return list;
    }

    public async Task<IEnumerable<TextValue<int>>> Handle(AvailableParticipantsQuery request, CancellationToken cancellationToken)
    {
      var list = await ctx.Student
                          .Where(s => !s.WorkshopParticipant.Where(wp => wp.WorkshopId == request.WorkshopId).Any())
                          .OrderBy(a => a.Surname)
                          .ThenBy(a => a.Name)
                          .Select(a => new TextValue<int>
                          {
                            Value = a.Id,
                            Text = $"{a.Surname}, {a.Name} ({a.Email})"
                          })
                          .ToListAsync(cancellationToken);
      return list;
    }
  }
}
