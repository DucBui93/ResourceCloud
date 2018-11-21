using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Business.Interfaces;
using RC.Models;
using RC.Models.Account.Request;
using RC.Models.Account.Response;
using RC.Models.Enums;

namespace RC.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerHelper
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Login with username and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<LoginResponse> LoginAsync([FromBody] LoginRequest request)
        {

            try
            {
                var response = await _userService.LoginAsync(request);

                return response;
            }
            catch (Exception e)
            {
                //_systemLogger.Error(e);
                return new LoginResponse
                {
                    ResponseMessage = new ResponseMessage(ResponseStatus.Status.FailWithException, e.Message)
                };
            };
        }

        // GET: api/User
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns> List user name </returns>
        [HttpGet]
        [Filter.Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/User/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
