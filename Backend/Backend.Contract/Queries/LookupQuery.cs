using Backend.Contract.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Backend.Contract.Queries
{
  public class LookupQuery<T, V> : IRequest<IEnumerable<TextValue<V>>>
  {
      
  } 
}
