using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RC.IdentityServerAPI.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var result = from c in User.Claims select new ReturnModel { Type = c.Type, Value = c.Value};
            return Ok(result);
        }

        public class ReturnModel
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}