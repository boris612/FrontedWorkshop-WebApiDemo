using MediatR;

namespace Backend.Core.Commands
{
  public class AddCommand<TDto> : IRequest<int>
  {
    public AddCommand(TDto dto)
    {
      Dto = dto;
    }
    public TDto Dto { get; set; }
  }
}
