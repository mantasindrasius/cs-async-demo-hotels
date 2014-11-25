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
        private readonly Collection<TaskDefinition> m_taskCollection;
        private readonly ToggleManager m_toggleManager;

        public TaskManager(ToggleManager toggleManager,
                           Collection<TaskDefinition> taskCollection)
        {
            m_toggleManager = toggleManager;
            m_taskCollection = taskCollection;
        }

        public Task ContinueWithTask(Task task, string description, Action action)
        {
            var regAction = RegisterTaskDef(description, action);

            return task.ContinueWith(t =>
            {
                System.Diagnostics.Debug.Print("Continue");

                regAction();
            });
        }

        public Task RunTask(string description, Action action)
        {
            var task = Task(description, action);
            task.Start();

            return task;
        }

        public Task Task(string description, Action action)
        {
            return new Task(RegisterTaskDef(description, action));
        }

        private Action RegisterTaskDef(string description, Action action)
        {
            var fakeLatency = GlobalRandom.Next(m_toggleManager.EmulatedLatency);

            var taskDef = new TaskDefinition(description, () =>
            {
                Thread.Sleep(fakeLatency);
                action();
            });

            m_taskCollection.Insert(0, taskDef);

            return taskDef.Action;
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
