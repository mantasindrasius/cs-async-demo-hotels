﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    public class ToggleManager
    {
        public int EmulatedLatency { get; set; }

        public ToggleManager()
        {
            EmulatedLatency = 1000;
        }
    }
}
