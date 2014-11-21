using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class Location
    {
        public String Id { get; private set; }
        public String Name { get; private set; }

        public Location(String id, String name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return String.Format("Location({0}, {1})", Id, Name);
        }
    }

    public class LocationAPI
    {
        private ILocationService service = new GoogleLocationService();

        private List<String> localities = new List<String>() {
            "Vilnius",
            "Kaunas",
            "Klaipėda",
            "Šiauliai",
            "Panevėžys",
            "Marijampolė",
            "Alytus",
            "Palanga",
            "Kretinga",
            "Utena",
            "Jurbarkas",
            "Ukmergė",
            "Šalčininkai"
        };

        private readonly List<Location> locations = new List<Location>();

        public LocationAPI()
        {
            var i = 0;

            locations = localities.Select(loc => new Location("loc" + (++i).ToString(), loc)).ToList();
        }

        public MapPoint GetLocation(String address)
        {
            return service.GetLatLongFromAddress(address);
        }

        public List<Location> GetAddressMatches(String addressText)
        {
            return locations.FindAll(s => s.Name.StartsWith(addressText));
        }
    }
}
