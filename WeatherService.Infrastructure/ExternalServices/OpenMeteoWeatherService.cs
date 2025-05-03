using WeatherService.Core.Interfaces;
using WeatherService.Core.Models;

namespace WeatherService.Infrastructure.Services
{

    public class OpenMeteoWeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiUrl = "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&hourly=temperature_2m";
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);

        public OpenMeteoWeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude)
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = RequestTimeout;

            try
            {
                var url = string.Format(ApiUrl, latitude, longitude);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var rawData = await response.Content.ReadAsStringAsync();
                return new WeatherData
                {
                    RetrievedAt = DateTime.UtcNow,
                    RawJson = rawData
                };
            }
            catch
            {
                return null;
            }
        }


    }
}
