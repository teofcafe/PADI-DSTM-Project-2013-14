using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace ServerLibrary
{
    public class TxCommitException : TxException
    {
        public TxCommitException(string message)
            : base(message)
        {

        } 
    }
}
