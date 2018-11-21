using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RC.API.Filter;

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
