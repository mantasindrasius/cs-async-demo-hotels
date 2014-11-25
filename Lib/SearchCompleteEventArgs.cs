using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class SearchCompleteEventArgs : EventArgs
    {
        public int ExecutionTime { get; private set; }

        public SearchCompleteEventArgs(int executionTime)
            : base()
        {
            ExecutionTime = executionTime;
        }
    }
}
