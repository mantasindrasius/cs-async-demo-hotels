using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace Lib.Test
{
    [TestClass]
    public class LocationAPITest
    {
        [TestMethod]
        public void GetLocation()
        {
            var api = new LocationAPI(null);

            var result = api.GetLocation("Vilnius");

            Assert.AreEqual(0, result.Latitude); 
        }

        [TestMethod]
        public void GetHotels()
        {
            var httpClient = new HttpClient();
            
            httpClient.BaseAddress = new Uri("http://hotels-world.fanta.wixpress.com/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var uri = new Uri("search?lan=50&lot=25&distance=30");

            //return m_toggleManager.AddLatency(() =>
            var respTask = httpClient.GetAsync("/api/search?lat=50&lon=25&distance=30");
            respTask.Wait();

            var resp = respTask.Result;

            //new List<Hotel>() {
            //    new Hotel("hotel-1", "Hotel 1"),
            //    new Hotel("hotel-2", "Hotel 2")
            //} );

            Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);

            if (resp.IsSuccessStatusCode)
            {
                var resultTask = resp.Content.ReadAsAsync<Hotel[]>(); //.ReadAsAsync<Hotel[]>();
                resultTask.Wait();

                Assert.IsTrue(resultTask.Result.Length == 0);
                //return ;
            } 
            //else
                //throw new InvalidOperationException("Request failed");

        }
    }
}
