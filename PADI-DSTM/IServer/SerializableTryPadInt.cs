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
    public class SerializableTryPadInt
    {
        public TimeStamp timeStamp = null;
        public int tempValue = 0;
        public int numberOfWaitingDependencies = 0;
        public TimeStamp[] dependencies = null;

        public SerializableTryPadInt(TimeStamp timeStamp, int value, int numberOfWaitingDependencies, LinkedList<TimeStamp> dependencies)
        {
            this.timeStamp = timeStamp;
            this.tempValue = value;
            this.numberOfWaitingDependencies = numberOfWaitingDependencies;
            this.dependencies = new TimeStamp[dependencies.Count];

            int i = 0;
            foreach (TimeStamp timestamp in dependencies)
                this.dependencies[i++] = timeStamp;
        }

        public SerializableTryPadInt(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.timeStamp = (TimeStamp)info.GetValue("timeStamp", typeof(TimeStamp));
            this.tempValue = info.GetInt32("tempValue");
            this.numberOfWaitingDependencies = info.GetInt32("numberOfWaitingDependencies");
            this.dependencies = (TimeStamp[])info.GetValue("dependencies", typeof(TimeStamp[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("timeStamp", this.timeStamp);
            info.AddValue("tempValue", this.tempValue);
            info.AddValue("numberOfWaitingDependencies", this.tempValue);
            info.AddValue("dependencies", this.tempValue);
        }
    }
}
