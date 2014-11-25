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
        public string Id { get; private set; }
        public string Name { get; private set; }

        public Location(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("Location({0}, {1})", Id, Name);
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
        private ToggleManager m_toggleManager;

        public LocationAPI(ToggleManager toggleManager)
        {
            var i = 0;

            m_toggleManager = toggleManager;
            locations = localities.Select(loc => new Location("loc" + (++i).ToString(), loc)).ToList();
        }

        public MapPoint GetLocation(string address)
        {
            return service.GetLatLongFromAddress(address);
        }

        public List<Location> GetAddressMatches(string addressText)
        {
            return m_toggleManager.AddLatency(() =>
            {
                return locations.FindAll(s => s.Name.StartsWith(addressText));
            });
        }
    }
}
