﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

namespace CoordinatorLibrary
{
    public interface PadInt
    {
        int Read();
        void Write(int value);
        string ToString();
    }
}
