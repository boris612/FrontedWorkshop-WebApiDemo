using Backend.Contract.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Backend.WebApi.Controllers
{
  [Authorize]
  public class UsersController : CrudController<User>
  {
   
  }
}
