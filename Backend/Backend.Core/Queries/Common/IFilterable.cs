using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Core.Queries.Common
{
  public interface IFilterable
  {
    List<Filter> Filters { get; set; }
  }
}
