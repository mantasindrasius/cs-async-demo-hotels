using EventbriteNET;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public class EventsAPI
    {
        public List<Event> GetEvents()
        {
            // Create the context object with your API details
            //var context = new EventbriteContext("APP_KEY", "USER_KEY");

            //http://www.eventbriteapi.com/v3/events/search?venue.city=Vilnius&start_date.range_start=2014-11-18T00:00:00Z&start_date.range_end=2014-11-20T00:00:00Z&token=C7ER7A5V42PKLVU2YP6H

            return new List<Event>() {
                new Event("Test 1", "Test desc", "2010-01-01 05:00-06:00", "Siemens arena", "http://www.americanlayout.com/wp/wp-content/uploads/2012/08/C-To-Go-300x300.png"),
                new Event("Test 2", "Test desc 2", "2010-01-01 05:00-06:00", "Kaunas arena", "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcSodMApp3rQxC0HGW2xhppbDz9AC5XAvxiU7MkSxhYlkX1A1NrAPf-sLg")
            };
        }
    }
}
