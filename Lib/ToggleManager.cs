using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lib
{
    public class ToggleManager
    {
        public bool AsyncOps { get; set; }
        public bool SequentialOps { get; set; }
        public bool AsyncSearchHotels { get; set; }
        public bool UserActionLogging { get; set; }
        public bool UserActionLoggingSequential { get; set; }

        public int EmulatedLatency { get; set; }

        public ToggleManager()
        {
            AsyncOps = true;
            EmulatedLatency = 1000;
        }

        public T AddLatency<T>(Func<T> f)
        {
            Thread.Sleep(EmulatedLatency);

            return f();
        }
    }
}
