using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
{
    [Serializable]
    public class Transaction : ISerializable
    {
        private TimeStamp timestamp;
        private string coordinatorURL;

        public Transaction(TimeStamp timestamp, string coordinatorURL)
        {
            this.timestamp = timestamp;
            this.coordinatorURL = coordinatorURL;
        }

        public Transaction(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            coordinatorURL = info.GetString("coordinatorUrl");
            timestamp = (TimeStamp)info.GetValue("timestamp", typeof(TimeStamp));
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
            info.AddValue("coordinatorUrl", coordinatorURL);
            info.AddValue("timestamp", timestamp);
        }
    }
}
