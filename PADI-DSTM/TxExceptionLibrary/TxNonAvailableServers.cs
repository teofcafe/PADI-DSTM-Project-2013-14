using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    [Serializable]
    public class TxNonAvailableServers : TxException
    {
        public TxNonAvailableServers(string message) : base(message) { }

        public TxNonAvailableServers(System.Runtime.Serialization.SerializationInfo info,
                              System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
