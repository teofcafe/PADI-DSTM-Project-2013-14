using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Transaction : MarshalByRefObject, ISerializable
    {
        private TimeStamp timestamp;
        private string ipCoordinator;

        public Transaction(TimeStamp timestamp, string ipCoordinator)
        {
            this.timestamp = timestamp;
            this.ipCoordinator = ipCoordinator;
        }

        public string ToString()
        {
            return "TimeStamp: " + timestamp.ToString() + ", Coordinator: " + ipCoordinator;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
