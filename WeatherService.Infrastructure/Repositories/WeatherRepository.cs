using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WeatherService.Core.Interfaces;
using WeatherService.Core.Models;
using WeatherService.Infrastructure.Data;

namespace WeatherService.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "LatestWeatherData";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1); // Cache for 1 hours

        public WeatherRepository(WeatherDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<WeatherData> GetLatestWeatherDataAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(CacheKey, out WeatherData cachedData))
            {
                return cachedData;
            }

            try
            {
                // If not in cache, get from database
                var data = await _context.WeatherData
                    .AsNoTracking()
                    .OrderByDescending(w => w.RetrievedAt)
                    .FirstOrDefaultAsync();

                if (data != null)
                {
                    _cache.Set(CacheKey, data, CacheDuration);
                }

                return data;
            }
            catch (Exception)
            {
                // Log.Error("Database get failed", ex);
            }

            return null;
       
        }

        public async Task SaveWeatherDataAsync(WeatherData weatherData)
        {
            try
            {
                _context.WeatherData.Add(weatherData);
                await _context.SaveChangesAsync();

                // Update cache
                _cache.Set(CacheKey, weatherData, CacheDuration);
            }
            catch (Exception)
            {
                // Log.Error("Database save failed", ex);
            }

        }
    }
}
