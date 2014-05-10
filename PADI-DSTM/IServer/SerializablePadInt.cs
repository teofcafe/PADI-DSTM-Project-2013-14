using PADI_DSTM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary
{
    [Serializable]
    public class SerializablePadInt
    {
        public int value;
        public int id;

        public TimeStamp lastSuccessfulCommit;
        public TimeStamp lastSuccessfulRead;
        public TimeStamp lastSuccessfulWrite;
        public bool preparedForCommit;

        public SerializablePadInt(int value, int id, TimeStamp lastSuccessfulCommit, 
                                    TimeStamp lastSuccessfulRead, TimeStamp lastSuccessfulWrite,
                                        bool preparedForCommit)
        {
            this.value = value;
            this.id = id;
            this.lastSuccessfulCommit = lastSuccessfulCommit;
            this.lastSuccessfulRead = lastSuccessfulRead;
            this.lastSuccessfulWrite = lastSuccessfulWrite;
            this.preparedForCommit = preparedForCommit;
        }

        public SerializablePadInt(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.value = info.GetInt32("value");
            this.id = info.GetInt32("id");
            this.lastSuccessfulCommit = (TimeStamp)info.GetValue("lastSuccessfulCommit", typeof(TimeStamp));
            this.lastSuccessfulRead = (TimeStamp)info.GetValue("lastSuccessfulRead", typeof(TimeStamp));
            this.lastSuccessfulWrite = (TimeStamp)info.GetValue("lastSuccessfulWrite", typeof(TimeStamp));
            this.preparedForCommit = info.GetBoolean("preparedForCommit");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", this.value);
            info.AddValue("id", this.id);
            info.AddValue("lastSuccessfulCommit", this.lastSuccessfulCommit);
            info.AddValue("lastSuccessfulRead", this.lastSuccessfulRead);
            info.AddValue("lastSuccessfulWrite", this.lastSuccessfulWrite);
            info.AddValue("preparedForCommit", this.preparedForCommit);
        }
    }
}
