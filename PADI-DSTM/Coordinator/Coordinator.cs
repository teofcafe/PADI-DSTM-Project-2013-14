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
        private const string endPoint = "Coordinator";
        private Transaction transaction = null;
        private LinkedList<IPadInt> transactionParticipants = new LinkedList<IPadInt>();

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

        public bool BeginTransaction(Transaction transaction)
        {
            this.transaction = transaction;
            System.Console.WriteLine("Coordinator.BeginTransaction() Called : " + transaction.ToString());
            return true;
        }

        public bool PrepareTransaction(Transaction transaction)
        {
            foreach (IPadInt padInt in transactionParticipants)
                padInt.PrepareCommit(transaction.TimeStamp);

            return true;
        }

        public bool CommitTransaction(Transaction transaction)
        {
            foreach (IPadInt padInt in transactionParticipants)
                padInt.Commit(transaction.TimeStamp);

            return true;
        }

        public bool AbortTransaction(Transaction transaction)
        {
            //TODO Abort

            return false;
        }


        public CoordinatorLibrary.PadInt CreatePadInt(int uid)
        {
            System.Console.Write("Coordinator.CreatePadInt() Called");

            IServer server = ServerConnector.GetServerResponsibleForObjectWithId(uid);
            ServerLibrary.IPadInt realPadInt = server.CreatePadInt(uid, transaction.TimeStamp);
            PadInt virtualPadInt = new PadInt(realPadInt, this.transaction.TimeStamp);
            this.transactionParticipants.AddFirst(realPadInt);

            return virtualPadInt;
        }

        public CoordinatorLibrary.PadInt AccessPadInt(int uid)
        {
            IServer server = ServerConnector.GetServerResponsibleForObjectWithId(uid);
            ServerLibrary.IPadInt realPadInt = server.AccessPadInt(uid, this.transaction.TimeStamp);
            PadInt virtualPadInt = new PadInt(realPadInt, this.transaction.TimeStamp);
            this.transactionParticipants.AddFirst(realPadInt);
            return virtualPadInt;
        }
    }
}
