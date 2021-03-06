﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PADI_DSTM
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

        public TimeStamp(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            timestamp = info.GetInt64("timestamp");
        }

        public override string ToString()
        {
            return timestamp.ToString();
        }

        public static bool operator <(TimeStamp t1, TimeStamp t2)
        {
            if (t1 == null)
                return true;
            else if (t2 == null)
                return false;
            else
                return t1.timestamp < t2.timestamp;
        }

        public static bool operator >(TimeStamp t1, TimeStamp t2)
        {
            if (t1 == null)
                return true;
            else if (t2 == null)
                return false;
            else
                return t1.timestamp > t2.timestamp;
        }

        public static bool operator >=(TimeStamp t1, TimeStamp t2)
        {
            if (t1 == null)
                return true;
            else if (t2 == null)
                return false;
            else
                return t1.timestamp >= t2.timestamp;
        }

        public static bool operator <=(TimeStamp t1, TimeStamp t2)
        {
            if (t1 == null)
                return true;
            else if (t2 == null)
                return false;
            else
                return t1.timestamp <= t2.timestamp;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("timestamp", timestamp);
        }

        public override bool Equals(object obj)
        {
            var item = obj as TimeStamp;

            if (item == null)
            {
                return false;
            }

            return this.timestamp == item.timestamp;
        }

        public override int GetHashCode()
        {
            return this.timestamp.GetHashCode();
        }
    }
}
