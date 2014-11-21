using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib
{
    public class TaskDefinition : INotifyPropertyChanged
    {
        public string Description { get; set; }
        public double? ExecutionTime { get; set; }
        public String Status { get; set; }
        public Action Action { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public TaskDefinition(string description, Action action)
        {
            SetPropertyValue("Description", description);
            SetPropertyValue("Status", "Pending");

            Action = () =>
            {
                var timeStarted = DateTime.Now;
                Thread.Sleep(GlobalRandom.Next(10000));

                try
                {
                    SetPropertyValue("Status", "Starting");
                    action();
                    SetPropertyValue("Status", "Finished");
                }
                catch
                {
                    SetPropertyValue("Status", "Error");
                }

                SetPropertyValue("ExecutionTime", DateTime.Now.Subtract(timeStarted).TotalMilliseconds);
            };
        }

        private void SetPropertyValue(string name, object value)
        {
            GetType().GetProperty(name).SetValue(this, value);

            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class TaskManager
    {
        private Collection<TaskDefinition> m_taskCollection;

        public TaskManager(Collection<TaskDefinition> taskCollection)
        {
            m_taskCollection = taskCollection;
        }

        public Task Task(string description, Action action)
        {
            var taskDef = new TaskDefinition(description, action);
            var task = new Task(taskDef.Action);

            m_taskCollection.Insert(0, taskDef);

            return task;
        }
    }

    class GlobalRandom
    {
        private static readonly Random random = new Random(DateTime.Now.Millisecond);

        public static int Next(int max)
        {
            lock (random)
            {
                return random.Next(max);
            }
        }
    }
}
