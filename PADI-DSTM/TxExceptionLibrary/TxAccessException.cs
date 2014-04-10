using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    [Serializable]
    public class TxAccessException : TxException
    {
        public TxAccessException(string message)
            : base(message)
        {

        }
    }
}

