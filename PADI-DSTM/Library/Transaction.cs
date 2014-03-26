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
        private string coordinatorURL;

        public Transaction(TimeStamp timestamp, string coordinatorURL)
        {
            this.timestamp = timestamp;
            this.coordinatorURL = coordinatorURL;
        }

        public TimeStamp TimeStamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        public string CoordinatorURL
        {
            get { return this.coordinatorURL; }
            set { this.coordinatorURL = value; }
        }

        public string ToString()
        {
            return "TimeStamp: " + timestamp.ToString() + ", Coordinator: " + coordinatorURL;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
