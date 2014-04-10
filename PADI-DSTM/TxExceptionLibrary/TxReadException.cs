using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
        [Serializable]
    public class TxReadException : TxException
    {
        public TxReadException(string message) : base(message) { }

        public TxReadException(System.Runtime.Serialization.SerializationInfo info,
                              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
