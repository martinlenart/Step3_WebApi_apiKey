using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Step3_WebApi_Jwt.Models;
using Step3_WebApi_Jwt.Services;

namespace Step3_WebApi_Jwt.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILoginService _loginService;
        private ILogger<LoginController> _logger;

        public LoginController(ILoginService userlogin, ILogger<LoginController> logger)
        {
            _loginService = userlogin;
            _logger = logger;

            _logger.LogInformation("LoginController started");
        }


        //POST: api/Login/LoginUser
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Guid))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult LoginUser([FromBody] UserCredentials userLogins)
        {
            _logger.LogInformation("LoginUsers initiated");

            if (_loginService.LoginUser(userLogins.UserName, userLogins.Password, out User user))
            {
                _logger.LogInformation("User logged in, apiKey sent");
                return Ok(user.apiKey);
            }

            _logger.LogWarning("wrong user or password");
            return BadRequest($"wrong user or password");
        }

        //GET: api/Login/LoggedInUsers
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult LoggedInUsers(string apiKey)
        {
            _logger.LogInformation("LoggedInUsers initiated");

            if (!_loginService.ValidateApiKey(apiKey, out _))
            {
                _logger.LogWarning("invalid apiKey");
                return BadRequest($"invalid apiKey");
            }

            _logger.LogInformation("User logged in");
            return Ok(_loginService.LoggedinUsers);
        }
    }
}