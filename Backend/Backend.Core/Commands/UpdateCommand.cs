using MediatR;

namespace Backend.Core.Commands
{
  public class UpdateCommand<TDto> : IRequest
  {
    public UpdateCommand(TDto dto)
    {
      Dto = dto;
    }
    public TDto Dto { get; set; }    
  }
}
