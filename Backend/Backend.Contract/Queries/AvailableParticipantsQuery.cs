using Backend.Contract.DTOs;

namespace Backend.Contract.Queries
{
  public class AvailableParticipantsQuery : LookupQuery<Student, int>
  {
    public int WorkshopId { get; set; }
  }
}
