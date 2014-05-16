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
        private ICoordinator coordinator;

        public PadInt(int padInt, TimeStamp timeStamp, ICoordinator coordinator)
        {
            this.padInt = padInt;
            this.timeStamp = timeStamp;
            this.coordinator = coordinator;
        }

        public int Read()
        {
            while (true)
            {
                try
                {
                    return ServerConnector.GetServerWithObjectWithId(padInt).AccessPadInt(padInt, timeStamp).ReplicatedRead(timeStamp);
                }
                catch (TxException e) {
                    this.coordinator.AbortTransaction(new Transaction(this.timeStamp, ""));
                    throw e;
                }
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
                catch (TxException e) {
                    this.coordinator.AbortTransaction(new Transaction(this.timeStamp, ""));
                    throw e;
                }
                catch (Exception) { }
            }
        }
    }
}
