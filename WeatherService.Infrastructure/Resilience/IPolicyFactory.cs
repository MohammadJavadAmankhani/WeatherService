using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService.Infrastructure.Resilience
{
    public interface IPolicyFactory
    {
        IAsyncPolicy<HttpResponseMessage> CreateHttpPolicy();
        IAsyncPolicy CreateDayabasePolicy();
    }
}
