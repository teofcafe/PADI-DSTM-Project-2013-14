using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary
{
    public static class MasterConnector
    {
        public const string MasterURL = "tcp://localhost:8089/Master";

        public static IMaster GetMaster() {
            return (IMaster)Activator.GetObject(typeof(IMaster), MasterURL);
        }
    }
}
