using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    public class TxWriteException : TxException
    {
        public TxWriteException(string message)
            : base(message)
        {

        }
    }
}
