using System.Collections.Generic;

namespace Backend.Core.Queries.Common
{
  public class SortInfo
  {
    public enum Order
    {
      ASCENDING, DESCENDING
    }
    /// <summary>
    /// Pair columnName, order
    /// </summary>
    public List<KeyValuePair<string, Order>> ColumnOrder { get; set; } = new List<KeyValuePair<string, Order>>();
  }
}
