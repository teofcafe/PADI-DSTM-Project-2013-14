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
using System.Collections;
using CoordinatorLibrary;
using TransactionLibrary;
using ServerLibrary;

namespace Coordinator
{
    public class Coordinator : MarshalByRefObject, ICoordinator
    {
        private static int nrServers;
        private const string endPoint = "Coordinator";
        private Transaction transaction;


        public Coordinator() {
            System.Console.WriteLine("Coordinator.Coordinator() Called");
        }

        public static void StartListening()
        {
            Console.WriteLine("Coordinator.StartListening() Called");

            try
            {
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(Coordinator),
                    Coordinator.endPoint,
                    WellKnownObjectMode.Singleton);

                Console.WriteLine("Coordinator Listening");
            }
            catch (Exception e)
            {
                Console.WriteLine("Coordinator NOT Listening!!!!!!");
                Console.WriteLine(e.ToString());
            }
        }

        public void CacheNotyfy(int nrServers) {
            //this.nrServers = nrServers;
        }

        public bool BeginTransaction(Transaction transaction)
        {
            this.transaction = transaction;
            System.Console.WriteLine("Coordinator.BeginTransaction() Called : " + transaction.ToString());
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


        public CoordinatorLibrary.PadInt CreatePadInt(int uid)
        {
            System.Console.Write("Coordinator.CreatePadInt() Called");

            IServer server = ServerConnector.GetServerResponsibleForObjectWithId(uid);

            ServerLibrary.IPadInt realPadInt = server.CreatePadInt(uid, transaction.TimeStamp);
            PadInt virtualPadInt = new PadInt(uid, this);

            return virtualPadInt;
        }

        public CoordinatorLibrary.PadInt CreateRealPadInt(int uid)
        {
            PadInt padint = new PadInt(uid, this);
            System.Console.Write("Criei PadInt");
            return padint;
        }

        public CoordinatorLibrary.PadInt AccessPadInt(int uid)
        {
        
            return null;
        }
    }
}
