using Microsoft.AspNetCore.Mvc;
using RC.API.Filter;
using System.Linq;

namespace RC.API.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class IdentityController : ControllerHelper
    {
        [HttpGet]
        public IActionResult Get()
        {   
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
