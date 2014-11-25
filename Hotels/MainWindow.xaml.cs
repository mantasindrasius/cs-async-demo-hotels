using Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Hotels
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ToggleManager m_toggleManager = new ToggleManager();

        private readonly LocationAPI m_location;
        private readonly HotelsAPI m_hotelsApi;
        private readonly WeatherAPI m_weatherApi;
        private readonly EventsAPI m_eventsApi;

        private readonly Toggler toggler;
        private readonly ActionLog actionLog;

        private readonly ObservableCollection<HotelRoom> m_rooms = new ObservableCollection<HotelRoom>();
        private readonly ObservableCollection<WeatherRecord> m_weather = new ObservableCollection<WeatherRecord>();
        private readonly ObservableCollection<Event> m_events = new ObservableCollection<Event>();
        private readonly ObservableCollection<TaskDefinition> m_tasks = new ObservableCollection<TaskDefinition>();
        private readonly TaskManager m_taskManager;
        private readonly ActionLogger m_actionLogger;
        //private readonly ObservableCollection<Room> m_weather = new ObservableCollection<Room>();
        //private readonly ObservableCollection<Room> m_rooms = new ObservableCollection<Room>();
        private readonly SearchManager m_searchManager;

        public MainWindow()
        {
            InitializeComponent();

            Closed += MainWindow_Closed;

            BindingOperations.EnableCollectionSynchronization(m_rooms, m_rooms);
            BindingOperations.EnableCollectionSynchronization(m_weather, m_weather);
            BindingOperations.EnableCollectionSynchronization(m_events, m_events);
            BindingOperations.EnableCollectionSynchronization(m_tasks, m_tasks);

            Rooms.ItemsSource = m_rooms;
            Weather.ItemsSource = m_weather;
            Events.ItemsSource = m_events;

            m_location = new LocationAPI(m_toggleManager);
            m_hotelsApi = new HotelsAPI(m_toggleManager);
            m_weatherApi = new WeatherAPI();
            m_eventsApi = new EventsAPI();

            toggler = new Toggler(m_toggleManager);
            actionLog = new ActionLog(m_tasks);

            //toggler.Show();
            //actionLog.Show();
                        
            m_taskManager = new TaskManager(m_toggleManager, m_tasks);
            m_searchManager = new SearchManager(m_taskManager, m_toggleManager, m_rooms, m_weather, m_events);
            m_actionLogger = new ActionLogger(m_taskManager, m_toggleManager);

            m_searchManager.StartingSearch += m_searchManager_StartingSearch;
            m_searchManager.SearchComplete += m_searchManager_SearchComplete;

            m_searchManager.Location = new Location("x", "y");
            m_searchManager.CheckIn = DateTime.Now;
            m_searchManager.CheckOut = DateTime.Now;

            PreviewMouseDown += MainWindow_PreviewMouseDown;
        }

        void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(null);

            m_actionLogger.Log(string.Format("Mouse clicked at {0}, {1}", Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y)));
        }

        void m_searchManager_SearchComplete(object sender, SearchCompleteEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ExecutionTime.Text = e.ExecutionTime.ToString() + "ms";
            });
        }

        void m_searchManager_StartingSearch(object sender, EventArgs e)
        {
            ExecutionTime.Text = "...";
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            toggler.Close();
            actionLog.Close();
        }

        private async void CBox_KeyUp(object sender, KeyEventArgs e)
        {
            var combo = (ComboBox)sender;
            var text = combo.Text;
            var addressTask = Task.Run(() => m_location.GetAddressMatches(text));
            
            var matches = await addressTask;

            combo.ItemsSource = matches;

            System.Diagnostics.Debug.Print("Search for {0}, num matches: {1}", text, matches.Count);
        }

        private void CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender;
            var item = (Location)combo.SelectedItem;

            m_searchManager.Location = item;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = (DatePicker)sender;

            switch (control.Name)
            {
                case "CheckIn":
                    m_searchManager.CheckIn = control.SelectedDate;
                    break;
                case "CheckOut":
                    m_searchManager.CheckOut = control.SelectedDate;
                    break;
            }
        }

        /*private Location Location
        {
            get { return (Location)CBox.SelectedItem; }
        }

        private DateTime? CheckInDate
        {
            get { return CheckIn.SelectedDate; }
        }

        private DateTime? CheckOutDate
        {
            get { return CheckOut.SelectedDate; }
        }

        private bool CanSearch
        {
            get { return Location != null && CheckInDate.HasValue && CheckOutDate.HasValue; }
        }*/

        private async void SearchSync_Click(object sender, RoutedEventArgs e)
        {
            var started = DateTime.Now;

            await Task.Run(() =>
            {
                ShowWeather();
                ShowEvents();

                var hotels = QueryHotels();

                ShowRooms(hotels);
            });

            ReportTimeTaken(DateTime.Now.Subtract(started).TotalMilliseconds);
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var started = DateTime.Now;
	
	        var tasks = new Task[] {
		        Task.Run(() => ShowWeather()),
		        Task.Run(() => ShowEvents()),
		        Task.Run(() => QueryHotels())
		          .ContinueWith(hotelsTask => ShowRooms(hotelsTask.Result)),
	        };

            await Task.WhenAll(tasks);

            ReportTimeTaken(DateTime.Now.Subtract(started).TotalMilliseconds);
        }

        private void ReportTimeTaken(double millis)
        {
            ExecutionTime.Text = millis + "ms";
        }

        private void ShowWeather()
        {
            m_weather.Clear();

            foreach (var weatherRecord in m_weatherApi.GetForecast())
            {
                m_weather.Add(weatherRecord);
            }
        }

        private void ShowEvents()
        {
            m_events.Clear();

            foreach (var anEvent in m_eventsApi.GetEvents())
            {
                m_events.Add(anEvent);
            }
        }

        private List<Hotel> QueryHotels()
        {
            return m_hotelsApi.GetHotels();
        }

        private void ShowRooms(List<Hotel> hotels)
        {
            m_rooms.Clear();

            Task.WaitAll(hotels.Select(h => ShowRooms(h)).ToArray());
        }
        
        private async Task ShowRooms(Hotel hotel)
        {
            var rooms = await Task.Run(() => m_hotelsApi.GetRooms(hotel));

            lock (m_rooms)
            {
                foreach (var room in rooms)
                    m_rooms.Add(new HotelRoom(room, hotel));
            }
        }
    }
}

