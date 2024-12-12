using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [Route("")]
        [HttpGet]
         public IActionResult GetConfig()
        {
            var config = new
            {
                AllowedHosts = _configuration["AllowedHosts"],
                Loglevel = _configuration["Logging:LogLevel:Default"],
                Environment = _configuration["ASPNETCORE_ENVIRONMENT"]

            };
            return Ok(config);

        }
    }
}
