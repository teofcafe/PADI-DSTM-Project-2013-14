using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace ServerLibrary
{
    public class TxAbortException : TxException
    {
        public TxAbortException(string message)
            : base(message)
        {

        }
    }
}

