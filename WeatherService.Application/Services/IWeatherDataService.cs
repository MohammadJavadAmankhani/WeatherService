using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService.Application.Services
{
    public interface IWeatherDataService
    {
        Task<string?> GetWeatherDataAsync(double latitude, double longitude);
    }
}
