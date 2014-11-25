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
        private readonly ToggleManager m_toggleManager;

        private volatile Task lastTask;

        public ActionLogger(TaskManager taskManager, ToggleManager toggleManager)
        {
            m_taskManager = taskManager;
            m_toggleManager = toggleManager;
        }

        public void Log(string description)
        {
            if (!m_toggleManager.UserActionLogging)
                return;

            var task = m_toggleManager.UserActionLoggingSequential && lastTask != null ?
                m_taskManager.ContinueWithTask(lastTask, description, () => { }) :
                m_taskManager.RunTask(description, () => { });

            lastTask = task;
        }
    }
}
