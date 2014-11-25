using GoogleMaps.LocationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

    public class Temperature
    {
        public double Celsius { get; set; }
    }

    public class DayForecast
    {
        public int Period { get; set; }
        public Temperature High { get; set; }
        public string Conditions { get; set; }
    }

    public class WeatherAPI
    {
        private HttpClient m_hotelsClient = Utils.MakeRestClient("http://api.wunderground.com/api/190e01642cadc217/forecast/q/");

        public List<WeatherRecord> GetForecast()
        {
            return Utils.UnwrapResult(GetForecastAsync());
        }

        public async Task<List<WeatherRecord>> GetForecastAsync()
        {
            var resp = await m_hotelsClient.GetAsync("Lithuania/Vilnius.json");
            var json = await resp.Content.ReadAsAsync<JObject>();

            var forecast = (JObject)json.GetValue("forecast");
            var simpleForecast = (JObject)forecast.GetValue("simpleforecast");
            var forecastDay = (JArray)simpleForecast.GetValue("forecastday");

            var forecasts = forecastDay
                .Select(n => n.ToObject<DayForecast>())
                .Take(3)
                .Select(f => new WeatherRecord(FormatTimeReference(f.Period), string.Format("{0}C", f.High.Celsius), f.Conditions))
                .ToList();

            return forecasts;
        }

        private string FormatTimeReference(int period)
        {
            switch (period)
            {
                case 1:
                    return "Today";
                case 2:
                    return "Tomorrow";
                default:
                    return DateTime.Today.AddDays(period - 1).DayOfWeek.ToString();
            }
        }

        public List<WeatherRecord> GetFakeForecast()
        {
            //http://api.wunderground.com/api/190e01642cadc217/forecast/q/Lithuania/Vilnius.json
            return new List<WeatherRecord>() {
                new WeatherRecord("Today", "12C", "Cloudy"),
                new WeatherRecord("Tomorrow", "24C", "Sunny"),
                new WeatherRecord("Tuesday", "30C", "Humid")
            };
        }

        //http://api.wunderground.com/api/Your_Key/geolookup/q/37.776289,-122.395234.json
    }
}
