using CoordinatorLibrary;
using ServerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionLibrary;

namespace Coordinator
{
    public class PadInt : MarshalByRefObject, CoordinatorLibrary.PadInt
    {
        private TimeStamp timeStamp;
        private IPadInt padInt;

        public PadInt(IPadInt padInt, TimeStamp timeStamp)
        {
            this.padInt = padInt;
            this.timeStamp = timeStamp;
        }

        public int Read()
        {
            return padInt.Read(timeStamp);
        }

        public void Write(int value)   {
            padInt.Write(value, timeStamp);
        }
    }
}
