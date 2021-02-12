using Backend.Core.Queries.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Core.Queries.Generic
{
  public class GetItemsQuery<TDto> : IRequest<IEnumerable<TDto>>, IPageable, ISortable
  {
    public SortOrder Sort { get; set; }
    public PagingData Paging { get; set; }
    public List<Filter> Filters { get; set; }
  }
}
