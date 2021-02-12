using MediatR;

namespace Backend.Core.Queries.Generic
{
  public class GetSingleItemQuery<TDto> : IRequest<TDto>, IHasIntegerId
  {
    public int Id { get; set; }
    public GetSingleItemQuery(int id)
    {
      Id = id;
    }
  }
}
