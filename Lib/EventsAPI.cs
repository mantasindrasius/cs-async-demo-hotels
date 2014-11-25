using EventbriteNET;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartsEnds { get; set; }
        public string Venue { get; set; }
        public string LogoUrl { get; set; }

        public Event(string name, string description, string startsEnds, string venue, string logoUrl)
        {
            Name = name;
            Description = description;
            StartsEnds = startsEnds;
            Venue = venue;
            LogoUrl = logoUrl;
        }
    }

    [DataContract]
    public class CompositeContent
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "html")]
        public string Html { get; set; }

    }

    [DataContract]
    public class EventTime
    {
        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }

        [DataMember(Name = "local")]
        public DateTime Local { get; set; }

        [DataMember(Name = "utc")]
        public DateTime Utc { get; set; }
    }

    [DataContract]
    public class EventDTO
    {
        [DataMember(Name = "name")]
        public CompositeContent Name { get; set; }

        [DataMember(Name = "logo_url")]
        public string LogoUrl { get; set; }

        [DataMember(Name = "description")]
        public CompositeContent Description { get; set; }

        [DataMember(Name = "start")]
        public EventTime Start { get; set; }

        [DataMember(Name = "end")]
        public EventTime End { get; set; }
    }

    public class EventsAPI
    {
        private HttpClient m_client = Utils.MakeRestClient("http://www.eventbriteapi.com/v3/events/");
        
        public List<Event> GetEvents()
        {
            return Utils.UnwrapResult(GetEventsAsync());
        }

        public async Task<List<Event>> GetEventsAsync()
        {
            var resp = await m_client.GetAsync("search?venue.city=Vilnius&start_date.range_start=2014-11-15T00:00:00Z&start_date.range_end=2014-11-30T00:00:00Z&token=C7ER7A5V42PKLVU2YP6H");

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException("Request failed");

            var json = await resp.Content.ReadAsAsync<JObject>();

            return ((JArray)json.GetValue("events"))
                    .Select(n => n.ToObject<EventDTO>())
                    .Take(5)
                    .Select(dto => new Event(dto.Name.Text, dto.Description.Text, dto.Start.Local.ToShortDateString(), dto.End.Local.ToShortDateString(), dto.LogoUrl))
                    .ToList();
        }

        public List<Event> GetFakeEvents()
        {
            // Create the context object with your API details
            //http://www.eventbriteapi.com/v3/events/search?venue.city=Vilnius&start_date.range_start=2014-11-18T00:00:00Z&start_date.range_end=2014-11-20T00:00:00Z&token=C7ER7A5V42PKLVU2YP6H


            return new List<Event>() {
                new Event("Test 1", "Test desc", "2010-01-01 05:00-06:00", "Siemens arena", "http://www.americanlayout.com/wp/wp-content/uploads/2012/08/C-To-Go-300x300.png"),
                new Event("Test 2", "Test desc 2", "2010-01-01 05:00-06:00", "Kaunas arena", "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcSodMApp3rQxC0HGW2xhppbDz9AC5XAvxiU7MkSxhYlkX1A1NrAPf-sLg")
            };
        }
    }
}
