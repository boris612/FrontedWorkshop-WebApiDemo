using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.WebApi.Models
{
  /// <summary>
  /// Map lazy loading parameters (e.g. from PrimeNG table)
  /// </summary>   
  public class LoadParams
  {    
    /// <summary>
    /// Starting row (i.e. skips First-1 rows)
    /// </summary>
    public int? First { get; set; }
    /// <summary>
    /// Number of elements to return
    /// </summary>
    public int? Rows { get; set; }
    /// <summary>
    /// Name of a column. Must be same as in corresponding DTO object, case insensitive
    /// </summary>
    public string Sort { get; set; }
    /// <summary>
    /// 1 ascending, -1 descending
    /// </summary>
    public int? SortOrder { get; set; }

    [BindNever] public bool Ascending => SortOrder == null || SortOrder != -1;

    /// <summary>
    /// Search filters. Each filter is like  key(operator)value, e.g. client(contains)Bob, agency(=)TNT, ...
    /// </summary>
    public string[] Filter { get; set; }

  }
}
