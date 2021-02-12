using Backend.Core;
using System.ComponentModel.DataAnnotations;

namespace Backend.Contract.DTOs
{
  public class School : IHasIntegerId
  {
    public int Id { get; set; }    
    [Required]
    public string Name { get; set; }
    public int TownId { get; set; }
    public string Town { get; set; }
  }
}
