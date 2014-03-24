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
        private Int32 timestamp;
        private string ipCoordinator;

        public Transaction(Int32 timestamp, string ipCoordinator)
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
