using Polly;
using WeatherService.Core.Interfaces;
using WeatherService.Core.Models;
using WeatherService.Infrastructure.Resilience;

namespace WeatherService.Infrastructure.Services
{

    public class OpenMeteoWeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiUrl = "https://api.ope-meteo.com/v1/forecst?latitude={0}&longitude={1}&hourly=temperature_2m";
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);
        private readonly IAsyncPolicy<HttpResponseMessage> _policy;

        public OpenMeteoWeatherService(IHttpClientFactory httpClientFactory,
            IPolicyFactory policyFactory)
        {
            _httpClientFactory = httpClientFactory;
            _policy = policyFactory.CreateHttpPolicy();
        }

        public async Task<WeatherData> GetWeatherDataAsync(double latitude, double longitude)
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = RequestTimeout;

            try
            {
                var url = string.Format(ApiUrl, latitude, longitude);
                var response = await _policy.ExecuteAsync(async () =>
                {
                    return await client.GetAsync(url);
                });

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
