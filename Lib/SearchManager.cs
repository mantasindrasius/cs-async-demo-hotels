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

        private readonly HotelsAPI m_hotelsApi = new HotelsAPI();
        private readonly WeatherAPI m_weatherApi = new WeatherAPI();
        private readonly EventsAPI m_eventsApi = new EventsAPI();

        public class SearchCompleteEventArgs : EventArgs
        {
            public int ExecutionTime { get; private set; }

            public SearchCompleteEventArgs(int executionTime)
                : base()
            {
                ExecutionTime = executionTime;
            }
        }

        public event EventHandler StartingSearch;
        public event EventHandler<SearchCompleteEventArgs> SearchComplete;

        class SearchCommand
        {
            private readonly HotelsAPI m_hotelsApi;
            private readonly WeatherAPI m_weatherApi;
            private readonly EventsAPI m_eventsApi;
            private readonly TaskManager m_taskManager;

            private readonly ICollection<HotelRoom> m_rooms;
            private readonly ICollection<WeatherRecord> m_weather;
            private readonly ICollection<Event> m_events;
            private readonly ICollection<Task> m_tasks = new List<Task>();
            private DateTime m_startTime;

            public event EventHandler<SearchCompleteEventArgs> Complete;

            public SearchCommand(TaskManager taskManager,
                                 HotelsAPI hotelsApi,
                                 WeatherAPI weatherApi,
                                 EventsAPI eventsApi,
                                 ICollection<HotelRoom> rooms,
                                 ICollection<WeatherRecord> weather,
                                 ICollection<Event> events)
            {
                m_taskManager = taskManager;
                m_hotelsApi = hotelsApi;
                m_weatherApi = weatherApi;
                m_eventsApi = eventsApi;
                m_rooms = rooms;
                m_weather = weather;
                m_events = events;
            }

            public void Execute()
            {
                DoExecute();

                Task.WaitAll(m_tasks.ToArray());
            }

            public Task ExecuteAsync()
            {
                return Task.Run(() => DoExecute());
            }

            private void DoExecute()
            {
                System.Diagnostics.Debug.Print("Search");

                m_startTime = DateTime.Now;

                var tasks = new Task[] {
                    SearchAllRooms(),
                    SearchWeather(),
                    SearchEvents()
                };

                foreach (var task in tasks)
                    task.Start();

                Task.Factory.ContinueWhenAll(tasks, (completed) =>
                {
                    if (Complete != null)
                    {
                        var millis = Convert.ToInt16((DateTime.Now - m_startTime).TotalMilliseconds);

                        Complete(this, new SearchCompleteEventArgs(millis));
                    }

                    System.Diagnostics.Debug.Print("Complete");
                });
            }

            private Task SearchAllRooms()
            {
                return new Task(() =>
                {
                    var tasks = m_hotelsApi.GetHotels().Select(hotel => SearchHotelRooms(hotel)).ToArray();

                    foreach (var task in tasks)
                        task.Start();

                    Task.WaitAll(tasks);
                });
            }

            private Task SearchHotelRooms(Hotel hotel)
            {
                return NewTask("Search hotel rooms", () =>
                {
                    foreach (var room in m_hotelsApi.GetRooms(hotel.Id))
                    {
                        lock (m_rooms)
                        {
                            m_rooms.Add(new HotelRoom(room, hotel));
                        }
                    }
                });
            }

            private Task SearchWeather()
            {
                return NewTask("Lookup weather info", () =>
                {
                    foreach (var room in m_weatherApi.GetForecast())
                    {
                        m_weather.Add(room);
                    }
                });
            }

            private Task SearchEvents()
            {
                return NewTask("Search local events", () =>
                {
                    foreach (var anEvent in m_eventsApi.GetEvents())
                    {
                        m_events.Add(anEvent);
                    }
                });
            }

            private Task NewTask(string description, Action action)
            {
                var task = m_taskManager.Task(description, action);
                m_tasks.Add(task);

                return task;
            }
        }

        public SearchManager(TaskManager taskManager,
                             ICollection<HotelRoom> rooms,
                             ICollection<WeatherRecord> weather,
                             ICollection<Event> events)
        {
            m_taskManager = taskManager;
            m_rooms = rooms;
            m_weather = weather;
            m_events = events;
        }

        public Location Location
        {
            set
            {
                m_location = value;
                Search();
            }
        }

        public DateTime? CheckIn
        {
            set
            {
                m_checkIn = value;
                Search();
            }
        }

        public DateTime? CheckOut
        {
            set
            {
                m_checkOut = value;
                Search();
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

        private void Search()
        {
            if (!CanSearch())
            {
                return;
            }

            Clear();

            if (StartingSearch != null)
                StartingSearch(this, EventArgs.Empty);

            var context = new SearchCommand(
                m_taskManager,
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

            context.ExecuteAsync();
        }
    }
}
