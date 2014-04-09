using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace ServerLibrary
{
    public class TxReadException : TxException
    {
        public TxReadException(string message) 
            : base(message)
        {
            
        }
    }
}
