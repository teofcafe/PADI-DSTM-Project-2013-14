using CoordinatorLibrary;
using ServerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

namespace Coordinator
{
    public class PadInt : MarshalByRefObject, CoordinatorLibrary.PadInt
    {
        private TimeStamp timeStamp;
        private int padInt;

        public PadInt(int padInt, TimeStamp timeStamp)
        {
            this.padInt = padInt;
            this.timeStamp = timeStamp;
        }

        public int Read()
        {
            while (true)
            {
                try
                {
                    return ServerConnector.GetServerWithObjectWithId(padInt).AccessPadInt(padInt, timeStamp).ReplicatedRead(timeStamp);
                }
                catch (TxException e) { throw e; }
                catch (Exception) { }
            }
        }

        public void Write(int value)
        {
            while (true)
            {
                try
                {
                    ServerConnector.GetServerWithObjectWithId(padInt).AccessPadInt(padInt, timeStamp).ReplicatedWrite(value, timeStamp);
                    return;
                }
                catch (TxException e) { throw e; }
                catch (Exception) { }
            }
        }
    }
}
