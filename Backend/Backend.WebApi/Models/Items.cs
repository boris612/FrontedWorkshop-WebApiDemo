using System.Collections.Generic;

namespace Backend.WebApi.Models
{
  public class Items<T>
  {
    public IEnumerable<T> Data { get; set; }
    public int Count { get; set; }
  }
}
