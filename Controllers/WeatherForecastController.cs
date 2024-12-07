using Azure.Core;
using BulkyWeb_Api.Authorization;
using BulkyWeb_Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulkyWeb_Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
    
    private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        [CheckPermission(Permission.Read)]
        public IEnumerable<WeatherForecast> Get()
        {
            
            var userName = User.Identity.Name;
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
