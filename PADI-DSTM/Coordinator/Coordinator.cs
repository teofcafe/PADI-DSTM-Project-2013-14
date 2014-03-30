using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Net.Sockets;

namespace Coordinator
{
    public class Coordinator : MarshalByRefObject, ICoordinator
    {
        private string coordinatorUrl;

        private const string endPoint = "/Coordinator";


        public Coordinator(String url, int port)
        {

            this.coordinatorUrl = url + port + Coordinator.endPoint;

            try
            {
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(Coordinator),
                    "Coordinator",
                    WellKnownObjectMode.SingleCall);
            }
            catch (Exception e)
            {

                Console.WriteLine("o coordinator nao esta a escuta" + e.ToString());
            }

            Console.WriteLine("Coordinator.Coordinator(): Got a new client!");
        }

        public Coordinator() {
            System.Console.WriteLine("CONSTRUTOR VAZIO CHAMADO!");
        }

            public bool BeginTransaction(Transaction transaction)
        {
            System.Console.WriteLine("BEGIN!");
            System.Console.WriteLine(sizeof(long));
            return true;
        }

        public bool PrepareTransaction(Transaction transaction)
        {
            return false;
        }

        public bool CommitTransaction(Transaction transaction)
        {
            return false;
        }

        public bool AbortTransaction(Transaction transaction)
        {
            return false;
        }


        public PadInt CreatePadInt(int uid)
        {
            return null;
        }

        public PadInt AccessPadInt(int uid)
        {
            return null;
        }
    }
}
