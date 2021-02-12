using Backend.Core.Queries.Common;
using MediatR;
using System.Collections.Generic;

namespace Backend.Core.Queries.Generic
{
  public class GetCountQuery<TDto> : IRequest<int>
  {
    public List<Filter> Filters { get; set; }
  }
}
