using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService.Infrastructure.Configuration
{
    public class ResilienceOptions
    {
        public const string SectionName = "Resilience";

        public RetryOptions Retry { get; set; }

        public CircuitBreakerOptions CircuitBreaker { get; set; }

        public TimeoutOptions Timeout { get; set; }
    }

    public class RetryOptions
    {
        public int MaxAttempts { get; set; } = 3;
        public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);
    }

    public class CircuitBreakerOptions
    {
        public int FailureThreshold { get; set; } = 5;
        public TimeSpan BreakDuration { get; set; } = TimeSpan.FromSeconds(30);
    }

    public class TimeoutOptions
    {
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
