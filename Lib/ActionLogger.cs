using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class ActionLogger
    {
        private readonly TaskManager m_taskManager;

        public ActionLogger(TaskManager taskManager)
        {
            m_taskManager = taskManager;
        }

        public void Log(string description)
        {
            m_taskManager.Task(description, () => { }).Start();
        }
    }
}
