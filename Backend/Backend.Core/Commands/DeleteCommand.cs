using MediatR;

namespace Backend.Core.Commands
{
  public class DeleteCommand<TDto> : IRequest
  {
    public int Id { get; set; }
    public DeleteCommand(int id)
    {
      Id = id;
    }
  }
}
