using AutoMapper;
using Backend.Core;
using Backend.Core.Commands;
using Backend.Core.Queries.Common;
using Backend.Core.Queries.Generic;
using Backend.Core.Util;
using Backend.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.WebApi.Controllers
{
  [Authorize]
  public abstract class CrudController<T> : BaseApiController where T: IHasIntegerId
  {

    /// <summary>
    ///  Get all items based on (lazy) load parameters (paging, sorting, and filtering)
    /// </summary>
    /// <param name="loadParams"></param>
    /// <param name="mediator"></param>
    /// <param name="mapper"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual async Task<Items<T>> GetAll([FromQuery] LoadParams loadParams, [FromServices] IMediator mediator, [FromServices] IMapper mapper)
    {
      var filters = FilterStringParser.Parse(loadParams.Filter);
      var result = new Items<T>();
      var countRequest = new GetCountQuery<T>
      {
        Filters = filters
      };
      result.Count = await mediator.Send(countRequest);

      if (result.Count > 0)
      {
        var dataRequest = new GetItemsQuery<T>
        {
          Paging = mapper.Map<LoadParams, PagingData>(loadParams),
          Sort = CreateSort(loadParams),
          Filters = filters
        };
        result.Data = await mediator.Send(dataRequest);
      }
      return result;      
    }    

    private SortOrder CreateSort(LoadParams loadParams)
    {
      SortOrder order = null;
      if (!string.IsNullOrWhiteSpace(loadParams.Sort))
      {
        order = new SortOrder();
        order.AddSortOrder(loadParams.Sort, ascending: loadParams.Ascending);
      }
      return order;
    }

    /// <summary>
    /// Returns single item based on primary key value
    /// </summary>
    /// <param name="id"></param>
    /// <param name="mediator"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<ActionResult<T>> Get(int id, [FromServices] IMediator mediator)
    {
      var query = new GetSingleItemQuery<T>(id);
      var item = await mediator.Send(query);
      if (item == null)
      {
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
      }
      else
      {
        return item;
      }      
    }


    /// <summary>
    /// Creates a new item.    
    /// </summary>
    /// <param name="model">id does not have to be sent (if sent it would be ignored)</param>
    /// <param name="mediator"></param>
    /// <returns>A newly created item</returns>
    /// <response code="201">Returns route to newly created item and sent data updated with id</response>
    /// <response code="400">If the model is null or not valid</response>  
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Create(T model, [FromServices] IMediator mediator)
    {
      var command = new AddCommand<T>(model);
      int id = await mediator.Send(command);

      var query = new GetSingleItemQuery<T>(id);
      var item = await mediator.Send(query);
      
      return CreatedAtAction(nameof(Get), new { id }, item);
    }

    /// <summary>
    /// Update the item
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <param name="mediator"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Update(int id, T model, [FromServices] IMediator mediator)
    {
      if (model.Id != id) //ModelState.IsValid & model != null checked automatically due to [ApiController]
      {
        return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids: {id} vs {model.Id}");
      }
      else
      {
        var query = new GetSingleItemQuery<T>(id);
        var item = await mediator.Send(query);
        if (item == null)
        {
          return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
        }

        var command = new UpdateCommand<T>(model);
        await mediator.Send(command);
        return NoContent();
      }
    }

    /// <summary>
    /// Delete the item base on primary key value (id)
    /// </summary>
    /// <param name="id">Primary key value</param>
    /// <param name="mediator">Query/Command (Request) mediator. (Obtained using Dependency Injection from services)</param>
    /// <returns></returns>
    /// <response code="204">If the item is deleted</response>
    /// <response code="404">If the item with id does not exist</response>      
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> Delete(int id, [FromServices] IMediator mediator)
    {      
      var query = new GetSingleItemQuery<T>(id);
      var item = await mediator.Send(query);
      if (item == null)
      {
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }

      var command = new DeleteCommand<T>(id);      
      await mediator.Send(command);
      return NoContent();
    }    
  }
}
