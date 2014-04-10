using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class TxPrepareException : TxException
    {
        public TxPrepareException(string message)
            : base(message)
        {

        }
    }
}
