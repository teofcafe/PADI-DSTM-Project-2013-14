using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [Serializable]
    public class TimeStamp : ISerializable
    {
        private long timestamp;

        public long Timestamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        public TimeStamp()
        {
            this.timestamp = DateTime.UtcNow.Ticks;
        }

        public TimeStamp(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            timestamp = info.GetInt64("timestamp");
        }

        public override string ToString()
        {
            return timestamp.ToString();
        }

        public static bool operator <(TimeStamp t1, TimeStamp t2)
        {
            return t1.timestamp<t2.timestamp;
        }

        public static bool operator >(TimeStamp t1, TimeStamp t2)
        {
            return t1.timestamp > t2.timestamp;
        }

        public static bool operator >=(TimeStamp t1, TimeStamp t2)
        {
            return t1.timestamp >= t2.timestamp;
        }

        public static bool operator <=(TimeStamp t1, TimeStamp t2)
        {
            return t1.timestamp <= t2.timestamp;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
          info.AddValue("timestamp", timestamp);   
        }
    }
}
