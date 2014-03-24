using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    class TimeStamp
    {
        private long timestamp = DateTime.UtcNow.Ticks;

        public long Timestamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
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
    }
}
