using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace ServerLibrary
{
    public class TxPrepareException : TxException
    {
        public TxPrepareException(string message)
            : base(message)
        {

        }
    }
}
