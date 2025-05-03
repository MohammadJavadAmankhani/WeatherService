using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherService.Core.Interfaces;

namespace WeatherService.Application.Services
{
    public class WeatherDataService : IWeatherDataService
    {
        private readonly IWeatherService _weatherService;
        private readonly IWeatherRepository _weatherRepository;

        public WeatherDataService(IWeatherService weatherService, IWeatherRepository weatherRepository)
        {
            _weatherService = weatherService;
            _weatherRepository = weatherRepository;
        }

        public async Task<string?> GetWeatherDataAsync(double latitude, double longitude)
        {
            try
            {
                // Try to get fresh data from the weather service
                var freshData = await _weatherService.GetWeatherDataAsync(latitude, longitude);

                if (freshData != null)
                {
                    // Save the fresh data
                    await _weatherRepository.SaveWeatherDataAsync(freshData);
                    return freshData.RawJson;
                }

                // If fresh data is not available, try to get the latest data from the database
                var latestData = await _weatherRepository.GetLatestWeatherDataAsync();
                return latestData?.RawJson;
            }
            catch (Exception)
            {
                // Fallback try to get the latest data from the database
                var latestData = await _weatherRepository.GetLatestWeatherDataAsync();

                return latestData?.RawJson;
            }
        }
    }
}
