using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class HotelRoom
    {
        public Room Room { get; private set; }
        public Hotel Hotel { get; private set; }

        public HotelRoom(Room room, Hotel hotel)
        {
            Room = room;
            Hotel = hotel;
        }
    }

    public class SearchManager
    {
        private Location m_location;
        private DateTime? m_checkIn;
        private DateTime? m_checkOut;

        private ICollection<HotelRoom> m_rooms;
        private ICollection<WeatherRecord> m_weather;
        private ICollection<Event> m_events;

        private readonly TaskManager m_taskManager;
        private readonly ToggleManager m_toggleManager;

        private readonly HotelsAPI m_hotelsApi;
        private readonly WeatherAPI m_weatherApi = new WeatherAPI();
        private readonly EventsAPI m_eventsApi = new EventsAPI();

        public event EventHandler StartingSearch;
        public event EventHandler<SearchCompleteEventArgs> SearchComplete;

        public SearchManager(TaskManager taskManager,
                             ToggleManager toggleManager,
                             ICollection<HotelRoom> rooms,
                             ICollection<WeatherRecord> weather,
                             ICollection<Event> events)
        {
            m_taskManager = taskManager;
            m_toggleManager = toggleManager;
            m_hotelsApi = new HotelsAPI(toggleManager);
            m_rooms = rooms;
            m_weather = weather;
            m_events = events;
        }

        public Location Location
        {
            set
            {
                m_location = value;
            }
        }

        public DateTime? CheckIn
        {
            set
            {
                m_checkIn = value;
            }
        }

        public DateTime? CheckOut
        {
            set
            {
                m_checkOut = value;
            }
        }

        private void Clear()
        {
            m_rooms.Clear();
            m_weather.Clear();
            m_events.Clear();
        }

        private bool CanSearch()
        {
            return !(m_checkIn == null || m_checkOut == null || m_location == null);
        }

        public async Task<bool> Search()
        {
            if (!CanSearch())
            {
                return false;
            }

            Clear();

            if (StartingSearch != null)
                StartingSearch(this, EventArgs.Empty);

            var context = new SearchCommand(
                m_taskManager,
                m_toggleManager,
                m_hotelsApi,
                m_weatherApi,
                m_eventsApi,
                m_rooms,
                m_weather,
                m_events);

            context.Complete += (sender, e) =>
            {
                if (SearchComplete != null) SearchComplete(this, e);
            };

            if (m_toggleManager.AsyncOps)
                await context.ExecuteAsync();
            else
                context.Execute();

            return true;
        }
    }
}
