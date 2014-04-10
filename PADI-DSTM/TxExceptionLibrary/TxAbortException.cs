using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class TxAbortException : TxException
    {
        public TxAbortException(string message)
            : base(message)
        {

        }
    }
}

