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
        private int uid;
        private ICoordinator coordinator;
        private TimeStamp timestamp;
        private IPadInt padint; 

        public PadInt(int uid, ICoordinator  coordinator)
        {
            this.uid = uid;
            this.coordinator = coordinator;
        }

        public int Read()
        {
            return padint.Read(timestamp);
        }

        public void Write(int value)   {
            padint.Write(value, timestamp);
        }

        public bool PrepareCommit(TransactionLibrary.TimeStamp timestamp)
        {
            throw new NotImplementedException();
        }

        public bool Commit(TransactionLibrary.TimeStamp timestamp)
        {
            throw new NotImplementedException();
        }
    }
}
