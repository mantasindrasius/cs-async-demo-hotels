using Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly LocationAPI m_location = new LocationAPI();
        private readonly Toggler toggler = new Toggler(new ToggleManager());
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

            actionLog = new ActionLog(m_tasks);

            toggler.Show();
            actionLog.Show();
                        
            m_taskManager = new TaskManager(m_tasks);
            m_searchManager = new SearchManager(m_taskManager, m_rooms, m_weather, m_events);
            m_actionLogger = new ActionLogger(m_taskManager);

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

        void m_searchManager_SearchComplete(object sender, SearchManager.SearchCompleteEventArgs e)
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

        private void CBox_KeyUp(object sender, KeyEventArgs e)
        {
            var combo = (ComboBox)sender;
            var text = combo.Text;
            var matches = m_location.GetAddressMatches(text);

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
    }

}

