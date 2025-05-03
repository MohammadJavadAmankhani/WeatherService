using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherService.Core.Models
{
    public class WeatherData
    {
        public int Id { get; set; }
        public DateTime RetrievedAt { get; set; }
        public string? RawJson { get; set; }
    }
}
