using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Net.Sockets;
using System.Collections;
using CoordinatorLibrary;
using PADI_DSTM;
using ServerLibrary;
using System.Collections.Concurrent;

namespace Coordinator
{
    public class Coordinator : MarshalByRefObject, ICoordinator
    {
        private const string endPoint = "Coordinator";
        private ConcurrentDictionary<TimeStamp, LinkedList<IPadInt>> transactionsToBeCommited = new ConcurrentDictionary<TimeStamp, LinkedList<IPadInt>>();

        public static void StartListening()
        {
            try
            {
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(Coordinator),
                    Coordinator.endPoint,
                    WellKnownObjectMode.Singleton);
            }
            catch (Exception e)
            {
                Console.WriteLine("Coordinator NOT Listening!!!!!!");
                Console.WriteLine(e.ToString());
            }
        }

        public bool BeginTransaction(Transaction transaction)
        {
            this.transactionsToBeCommited[transaction.TimeStamp] = new LinkedList<IPadInt>();

            return true;
        }

        public bool PrepareTransaction(Transaction transaction)
        {
           try
            {
               foreach (IPadInt padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                padInt.PrepareCommit(transaction.TimeStamp);

            return true;
            }
           catch (TxException e)
           {
               foreach (IPadInt padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                   padInt.Abort(transaction.TimeStamp);

               throw e;
           } 
        }

        public bool CommitTransaction(Transaction transaction)
        {
            
                foreach (IPadInt padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                    padInt.Commit(transaction.TimeStamp);

                LinkedList<IPadInt> removedIPadInt;
                this.transactionsToBeCommited.TryRemove(transaction.TimeStamp, out removedIPadInt);

                return true;          
        }

        public bool AbortTransaction(Transaction transaction)
        {
            foreach (IPadInt padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                padInt.Abort(transaction.TimeStamp);

            LinkedList<IPadInt> removedIPadInt;
            this.transactionsToBeCommited.TryRemove(transaction.TimeStamp, out removedIPadInt);
            return true;
        }


        public CoordinatorLibrary.PadInt CreatePadInt(int uid, Transaction transaction)
        {
            IServer server = ServerConnector.GetServerResponsibleForObjectWithId(uid);
            ServerLibrary.IPadInt realPadInt = server.CreatePadInt(uid, transaction.TimeStamp);
            PadInt virtualPadInt = new PadInt(realPadInt, transaction.TimeStamp);
            this.transactionsToBeCommited[transaction.TimeStamp].AddFirst(realPadInt);

            return virtualPadInt;
        }

        public CoordinatorLibrary.PadInt AccessPadInt(int uid, Transaction transaction)
        {
            IServer server = ServerConnector.GetServerResponsibleForObjectWithId(uid);
            ServerLibrary.IPadInt realPadInt = server.AccessPadInt(uid, transaction.TimeStamp);
            PadInt virtualPadInt = new PadInt(realPadInt, transaction.TimeStamp);
            this.transactionsToBeCommited[transaction.TimeStamp].AddFirst(realPadInt);
            return virtualPadInt;
        }

        public bool Status()
        {
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("----------------------COORDINATOR STATUS------------------------");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("Nr of coordinated transactions: " + transactionsToBeCommited.Count);
            if (transactionsToBeCommited.Count > 0)
            {
                Console.Write("     TimeStamp(s): { ");
                foreach (TimeStamp timestamp in transactionsToBeCommited.Keys)
                    Console.Write(timestamp.ToString() + " ");
                Console.WriteLine("}");
            }
            return true;
        }
    }
}
