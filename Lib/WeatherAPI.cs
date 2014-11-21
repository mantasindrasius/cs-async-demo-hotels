using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class WeatherRecord
    {
        public string TimeReference { get; set; }
        public string Temperature { get; set; }
        public string Conditions { get; set; }

        public WeatherRecord(string timeReference, string temperature, string conditions)
        {
            TimeReference = timeReference;
            Temperature = temperature;
            Conditions = conditions;
        }
    }

    public class WeatherAPI
    {
        public List<WeatherRecord> GetForecast()
        {
            return new List<WeatherRecord>() {
                new WeatherRecord("Today", "12C", "Cloudy"),
                new WeatherRecord("Tomorrow", "24C", "Sunny"),
                new WeatherRecord("Tuesday", "30C", "Humid")
            };
        }

        //http://api.wunderground.com/api/Your_Key/geolookup/q/37.776289,-122.395234.json
    }
}
