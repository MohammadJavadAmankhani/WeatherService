using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherService.Infrastructure.Configuration;

namespace WeatherService.Infrastructure.Resilience
{
    public class PolicyFactory : IPolicyFactory
    {
        private readonly ResilienceOptions _options;
        private readonly ILogger<PolicyFactory> _logger;

        public PolicyFactory(IOptions<ResilienceOptions> options,
            ILogger<PolicyFactory> logger)
        {
            _options = options.Value;
            _logger = logger;
        }
        public IAsyncPolicy CreateDayabasePolicy()
        {
            return Policy
                 .Handle<SqlException>()
                 .Or<TimeoutException>()
                 .WaitAndRetryAsync(
                     retryCount: _options.Retry.MaxAttempts,
                     sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }

        public IAsyncPolicy<HttpResponseMessage> CreateHttpPolicy()
        {
            var retryPolicy = HttpPolicyExtensions
               .HandleTransientHttpError()
               .WaitAndRetryAsync(
                   retryCount: _options.Retry.MaxAttempts,
                   sleepDurationProvider: retryAttempt =>
                       TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                   onRetry: (outcome, timespan, retryCount, context) =>
                   {
                       _logger.LogWarning("Http retry {RetryCount} after {Delay}ms",
                           retryCount, timespan.TotalMilliseconds);
                   });

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: _options.CircuitBreaker.FailureThreshold,
                    durationOfBreak: _options.CircuitBreaker.BreakDuration,
                    onBreak: (result, duration) =>
                    {
                        _logger.LogWarning("Circuit breaker opened for {Duration}ms", duration.TotalMilliseconds);
                    },
                    onReset: () => _logger.LogInformation("Circuit breaker reset"));

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(_options.Timeout.RequestTimeout);

            return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
        }
    }
}
