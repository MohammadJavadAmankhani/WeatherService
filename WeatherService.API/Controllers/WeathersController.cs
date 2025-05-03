using Microsoft.AspNetCore.Mvc;
using WeatherService.Application.Services;
using WeatherService.Core.Models;

namespace WeatherService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeathersController : ControllerBase
    {
        private readonly WeatherDataService _weatherDataService;

        public WeathersController(WeatherDataService weatherDataService)
        {
            _weatherDataService = weatherDataService;
        }

        [HttpGet]
        public async Task<ActionResult> GetWeatherData([FromQuery] double latitude = 52.52, [FromQuery] double longitude = 13.41)
        {
            var weatherData = await _weatherDataService.GetWeatherDataAsync(latitude, longitude);

            return Ok(weatherData);
        }
    }
}
