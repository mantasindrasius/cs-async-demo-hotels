using Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hotels
{
    /// <summary>
    /// Interaction logic for ActionLog.xaml
    /// </summary>
    public partial class ActionLog : Window
    {
        private ObservableCollection<TaskDefinition> m_tasks;

        public ActionLog(ObservableCollection<TaskDefinition> tasks)
        {
            InitializeComponent();

            Tasks.ItemsSource = tasks;
        }
    }
}
