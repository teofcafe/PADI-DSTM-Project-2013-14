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
using MasterLibrary;

namespace Coordinator
{
    public class Coordinator : MarshalByRefObject, ICoordinator
    {
        private const string endPoint = "Coordinator";
        private ConcurrentDictionary<TimeStamp, LinkedList<int>> transactionsToBeCommited = new ConcurrentDictionary<TimeStamp, LinkedList<int>>();

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
            this.transactionsToBeCommited[transaction.TimeStamp] = new LinkedList<int>();

            return true;
        }

        public bool PrepareTransaction(Transaction transaction)
        {
            Dictionary<int, int> objectsConfirmed = new Dictionary<int, int>();
            bool abortPhase = false;

            while (true)
            {
                try
                {
                    if (abortPhase)
                    {
                        foreach (int padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                        {
                            if (objectsConfirmed.ContainsKey(padInt)) continue;
                            ServerConnector.GetServerWithObjectWithId(padInt).AccessPadInt(padInt, transaction.TimeStamp).ReplicateAbort(transaction.TimeStamp);
                            objectsConfirmed[padInt] = padInt;
                        }

                        return false;
                    }
                    else
                    {
                        try
                        {
                            foreach (int padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                            {
                                if (objectsConfirmed.ContainsKey(padInt)) continue;
                                ServerConnector.GetServerWithObjectWithId(padInt).AccessPadInt(padInt, transaction.TimeStamp).ReplicatedPrepareCommit(transaction.TimeStamp);
                                objectsConfirmed[padInt] = padInt;
                            }

                            return true;
                        }
                        catch (TxException)
                        {
                            abortPhase = true;
                            objectsConfirmed.Clear();
                        }
                    }
                }
                catch (TxException e) { throw e; }
                catch (Exception)
                {

                }
            }
        }

        public bool CommitTransaction(Transaction transaction)
        {
            Dictionary<int, int> objectsConfirmed = new Dictionary<int, int>();

            while (true)
            {
                try
                {
                    foreach (int padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                    {
                        if (objectsConfirmed.ContainsKey(padInt)) continue;
                        ServerConnector.GetServerWithObjectWithId(padInt).AccessPadInt(padInt, transaction.TimeStamp).ReplicateCommit(transaction.TimeStamp);
                        objectsConfirmed[padInt] = padInt;
                    }

                    LinkedList<int> removedIPadInt;
                    this.transactionsToBeCommited.TryRemove(transaction.TimeStamp, out removedIPadInt);

                    return true;
                }
                catch (TxException e) { throw e; }
                catch (Exception) { }
            }
        }

        public bool AbortTransaction(Transaction transaction)
        {
            Dictionary<int, int> objectsConfirmed = new Dictionary<int, int>();

            while (true)
            {
                try
                {
                    foreach (int padInt in this.transactionsToBeCommited[transaction.TimeStamp])
                    {
                        if (objectsConfirmed.ContainsKey(padInt)) continue;
                        ServerConnector.GetServerWithObjectWithId(padInt).AccessPadInt(padInt, transaction.TimeStamp).ReplicateAbort(transaction.TimeStamp);
                        objectsConfirmed[padInt] = padInt;
                    }

                    LinkedList<int> removedIPadInt;
                    this.transactionsToBeCommited.TryRemove(transaction.TimeStamp, out removedIPadInt);

                    return true;
                }
                catch (TxException e) { throw e; }
                catch (Exception) { }
            }
        }


        public CoordinatorLibrary.PadInt CreatePadInt(int uid, Transaction transaction)
        {
            while (true)
            {
                try
                {
                    IServer server = ServerConnector.GetServerResponsibleForObjectWithId(uid);
                    ServerLibrary.IPadInt realPadInt = server.CreateReplicatedPadInt(uid, transaction.TimeStamp);
                    PadInt virtualPadInt = new PadInt(uid, transaction.TimeStamp, this);
                    this.transactionsToBeCommited[transaction.TimeStamp].AddFirst(realPadInt.Id);

                    return virtualPadInt;
                }
                catch (TxException e) { throw e; }
                catch (Exception) { }
            }
        }

        public CoordinatorLibrary.PadInt AccessPadInt(int uid, Transaction transaction)
        {
            while (true)
            {
                try
                {
                    IServer server = ServerConnector.GetServerWithObjectWithId(uid);
                    ServerLibrary.IPadInt realPadInt = server.ReplicatedAccessPadInt(uid, transaction.TimeStamp);
                    PadInt virtualPadInt = new PadInt(uid, transaction.TimeStamp, this);
                    this.transactionsToBeCommited[transaction.TimeStamp].AddFirst(realPadInt.Id);

                    return virtualPadInt;
                }
                catch (TxException e) { throw e; }
                catch (Exception) { }
            }

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
