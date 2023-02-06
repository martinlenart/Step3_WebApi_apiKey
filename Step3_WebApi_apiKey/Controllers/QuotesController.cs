using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;

using Step3_WebApi_WebApi.Models;
using Step3_WebApi_Jwt.Services;
using Microsoft.AspNetCore.Authorization;

namespace Step3_WebApi_Jwt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private ILoginService _loginService;
        private IMockupData _repo;
        private ILogger<QuotesController> _logger;

        //GET: api/Quotes/?apikey={apiKey}
        //GET: api/Quotes/?count={count}&apikey={apiKey}
        //Below are good practice decorators to use for a GET request
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GoodQuote>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public IActionResult Get(string count, string apiKey)
        {
            _logger.LogInformation("ApiKeyGetQuotes initiated");
            if (!_loginService.ValidateApiKey(apiKey, out _))
            {
                _logger.LogWarning("invalid apiKey");
                return BadRequest($"invalid apiKey");
            }

            if (string.IsNullOrWhiteSpace(count))
            {
                _logger.LogInformation("GetQuotes returned {count} items", _repo.Quotes.Count);
                return Ok(_repo.Quotes);
            }

            if (!int.TryParse(count, out int _count))
            {
                return BadRequest("count format error");
            }

            _count = Math.Min(_count, _repo.Quotes.Count);            
            _logger.LogInformation("GetQuotes returned {_count} items", _count);
            return Ok(_repo.Quotes.Take(_count));
        }

        public QuotesController(IMockupData repo, ILogger<QuotesController> logger, ILoginService loginService)
        {
            _repo = repo;
            _logger = logger;
            _loginService = loginService;

            _logger.LogInformation($"QuotesController started.");
        }
    }
}