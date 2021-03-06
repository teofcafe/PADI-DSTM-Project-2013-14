﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    [Serializable]
    public class TxAbortException : TxException
    {
        public TxAbortException(string message) : base(message) { }

        public TxAbortException(System.Runtime.Serialization.SerializationInfo info,
                              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

