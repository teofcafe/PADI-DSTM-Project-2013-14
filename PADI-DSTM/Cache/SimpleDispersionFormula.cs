using DispersionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DispersionLibrary
{
    [Serializable]
    public class SimpleDispersionFormula : IDispersionFormula, ISerializable
    {
        private int numberOfServers = 0;

        public int NumberOfServers
        {
            get { return this.numberOfServers; }
            set { this.numberOfServers = value; }
        }

        public SimpleDispersionFormula() { }

        public int GetIdOfServerWithObjectWithId(int uid)
        {
            return (uid % numberOfServers);
        }

        public int GetIdOfReplicaServerWithObjectWithId(int uid)
        {
            return ((uid + 1) % numberOfServers);
        }

        public SimpleDispersionFormula(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) {
            numberOfServers = info.GetInt32("numberOfServers");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("numberOfServers", numberOfServers);
        }
    }
}
