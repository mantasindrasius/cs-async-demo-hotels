using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Diagnostics;

namespace Lib.Test
{
    [TestClass]
    public class WeatherAPITest
    {
        [TestMethod]
        public void GetWeatherTest()
        {
            var api = new WeatherAPI();
            //var hotel = new Hotel("hotel-1", "Hotel 1", "Ct8l4ZzadI7YwqtHvun8xB482Nl81FzsDeOGThl2Wtc.eyJpbnN0YW5jZUlkIjoiMTM4ZTAwZWEtMzI4NC04ODY5LWViYmMtMDMxNGQ4ODU0NTQ5Iiwic2lnbkRhdGUiOiIyMDE0LTExLTI1VDA2OjM4OjAxLjI1MloiLCJ1aWQiOiJmZGUwMTUxMi04ZWVkLTRmNDItODc4Zi1iODkxYTdhMWJlNjYiLCJwZXJtaXNzaW9ucyI6Ik9XTkVSIiwiaXBBbmRQb3J0IjoiODguMTE5LjE1MC4xOTYvMzQ4MDEiLCJ2ZW5kb3JQcm9kdWN0SWQiOm51bGwsImRlbW9Nb2RlIjpmYWxzZX0");

            var forecast = api.GetForecast();

            Assert.IsTrue(forecast.Count > 0);
        }
    }
}
