using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
        [Serializable]
    public class TxWriteException : TxException
    {
        public TxWriteException(string message) : base(message) { }

        public TxWriteException(System.Runtime.Serialization.SerializationInfo info,
                              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
