using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    class SearchCommand
    {
        private readonly HotelsAPI m_hotelsApi;
        private readonly WeatherAPI m_weatherApi;
        private readonly EventsAPI m_eventsApi;
        private readonly TaskManager m_taskManager;
        private readonly ToggleManager m_toggleManager;

        private readonly ICollection<HotelRoom> m_rooms;
        private readonly ICollection<WeatherRecord> m_weather;
        private readonly ICollection<Event> m_events;
        private readonly ICollection<Task> m_tasks = new List<Task>();
        private DateTime m_startTime;

        public event EventHandler<SearchCompleteEventArgs> Complete;

        public SearchCommand(TaskManager taskManager,
                             ToggleManager toggleManager,
                             HotelsAPI hotelsApi,
                             WeatherAPI weatherApi,
                             EventsAPI eventsApi,
                             ICollection<HotelRoom> rooms,
                             ICollection<WeatherRecord> weather,
                             ICollection<Event> events)
        {
            m_taskManager = taskManager;
            m_toggleManager = toggleManager;
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
                SearchWeather(),
                SearchEvents(),
                SearchAllRooms(!m_toggleManager.AsyncSearchHotels),
            };

            RunTasks(tasks, m_toggleManager.SequentialOps);

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

        private Task SearchAllRooms(bool isSync = false)
        {
            //return new Task(() =>
            //{
                /*if (isSync)
                {
                    var hotels = 
                    foreach (var hotel in hotels)
                        SearchHotelRooms(hotel).RunSynchronously();
                }
                else
                {
                    var tasks = m_hotelsApi.GetHotels().Select(hotel => SearchHotelRooms(hotel)).ToArray();

                    RunTasks(tasks);

                    Task.WaitAll(tasks);
                }*/

                throw new NotImplementedException();
            //});
        }

        private Task SearchHotelRooms(Hotel hotel)
        {
            return NewTask("Search hotel rooms", () =>
            {
                foreach (var room in m_hotelsApi.GetRooms(hotel))
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

        private void RunTasks(Task[] tasks, bool isSync = false)
        {
            foreach (var task in tasks)
                if (isSync)
                    task.RunSynchronously();
                else
                    task.Start();
        }
    }
}
