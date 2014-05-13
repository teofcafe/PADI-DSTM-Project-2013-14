using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coordinator;
using System.Threading;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Net.Sockets;
using ServerLibrary;
using CoordinatorLibrary;
using MasterLibrary;
using PADI_DSTM;
using System.Runtime.Serialization.Formatters;
using System.Collections.Concurrent;

namespace Server
{
    public class Server : MarshalByRefObject, IServer, PadInt.Callback
    {

        private static int id, nrServers;
        private static bool failed = false;
        private static bool freezed = false;

        //Carga maxima do server #Coordenar transaccao -> peso 1, #Read -> peso 2, #Write -> peso 3
        private int maxCharge, actualCharge;
        private bool overCharged;

        private Dictionary<int, PadInt> repository = new Dictionary<int, PadInt>();
        //hashtable[uid=4] = PadInt with value 4;

        //store the received special objects on this structure
        private ConcurrentQueue<PadInt> specialObjects = new ConcurrentQueue<PadInt>();
        private ConcurrentQueue<PadInt> objectsToReplicate = new ConcurrentQueue<PadInt>();
        private ConcurrentQueue<PadInt> objectsToMigrate = new ConcurrentQueue<PadInt>();
        // If the Master refuse the request of data migration, save the uids in this structure to migrate later. Ordered by nr of accesses.
        //Migrate only if 2 elements exists
        private Dictionary<int, PadInt> receivedSpecialObjects = new Dictionary<int, PadInt>();

        private const string url = "tcp://localhost";

        private const string endPoint = "Server";
        private static int serverPort;

        private const string masterUrl = "tcp://localhost:8089/Master";

        public static void StartListening(string url, int port)
        {
            serverPort = port;

            ChannelServices.RegisterChannel(new TcpChannel(port), true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Server),
                Server.endPoint,
                WellKnownObjectMode.Singleton);

            Coordinator.Coordinator.StartListening();

            IMaster master = MasterConnector.GetMaster();
            Server.id = master.RegisterServer(url + ":" + port);
        }

        public Server()
        {
            //ThreadStart startDelegate = new ThreadStart(VerifyCharge);
            //Thread threadOne = new Thread(startDelegate);
            //threadOne.Priority = ThreadPriority.Lowest;
            PadInt.callbackServer = this;
            ThreadStart startDelegate = new ThreadStart(() =>
            {
                while (true)
                {
                    lock (objectsToReplicate)
                    {
                        Monitor.Wait(objectsToReplicate);

                    }
                    while (!objectsToReplicate.IsEmpty)
                    {
                        PadInt toReplicate;
                        if (objectsToReplicate.TryDequeue(out toReplicate))
                        {
                            try
                            {
                                ((Server)ServerConnector.GetReplicationServerForObjectWithId(toReplicate.Id)).UpdatePadInt(toReplicate.ToSerializablePadInt());
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                                objectsToReplicate.Enqueue(toReplicate);
                            }
                        }

                        Thread.Sleep(1000);
                    }

                }
            });

            ThreadStart migrationThread = new ThreadStart(() =>
            {
                Console.WriteLine("Started!!!!!");
                while (true)
                {
                    lock (this.specialObjects)
                    {
                        Monitor.Wait(this.specialObjects);
                    }

                    while (this.specialObjects.Count > 1)
                    {
                        PadInt toMigrate;
                        if (this.specialObjects.TryDequeue(out toMigrate))
                        {
                            try
                            {
                                string serverToMigrateTo = MasterConnector.GetMaster().GetServerURLToMigrate(toMigrate.Id);
                                Console.WriteLine("Tentar migrar " + toMigrate.Id + " para " + serverToMigrateTo);

                                if (!ServerConnector.GetServerWithURL(serverToMigrateTo + "/" + ServerConnector.ServerEndPoint).ReceiveSpecialPadInt(toMigrate.ToSerializablePadInt()))
                                {
                                    this.specialObjects.Enqueue(toMigrate);
                                    Console.WriteLine("IF");
                                }
                                else
                                {
                                    repository.Remove(toMigrate.Id);
                                }
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("else: " + e.Message);
                                this.specialObjects.Enqueue(toMigrate);
                            }
                        }

                        Thread.Sleep(1000);
                    }

                }
            });

            Thread threadOne = new Thread(startDelegate);
            threadOne.Priority = ThreadPriority.Lowest;

            threadOne.Start();

            Thread threadTwo = new Thread(migrationThread);
            threadTwo.Priority = ThreadPriority.Lowest;

            threadTwo.Start();
        }

        public bool VerifyOverCharge()
        {
            return actualCharge > maxCharge;
        }

        public bool ReceiveSpecialPadInt(SerializablePadInt padInt)
        {
            Console.WriteLine("Special with id: {0} PadInt Received.", padInt.id);
            PadInt realPadInt = new PadInt(padInt);

            try
            {
                if (!receivedSpecialObjects.ContainsKey(realPadInt.Id))
                    receivedSpecialObjects[realPadInt.Id] = realPadInt;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            
            return true;
        }

        static void Main(string[] args)
        {
            int port;

            Console.Write("Port: ");
            if (!int.TryParse(Console.ReadLine(), out port))
            {
                Console.WriteLine("Invalid Port!!!!!");
                port = new Random().Next(2000) + 1000;
            }


            Server.StartListening("tcp://localhost", port);

            Console.WriteLine("Listening on URL: {0}:{1}", "tcp://localhost", port);
            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }

        public void UpdatePadInt(SerializablePadInt serPadInt)
        {

        }


        public void RemovePadInt(PadInt padint)
        {
            this.repository.Remove(padint.Id);
        }

        public bool IsFreezed()
        {
            return freezed;
        }

        public bool IsFailed()
        {
            return failed;
        }

        public string GetUrl()
        {
            return url + ":" + serverPort;
        }


        public void DangerAcess(PadInt padint)
        {
            Console.WriteLine("DANGER CHEGUEI AQUI com o ID" + padint.Id);
            try
            {
                Migrate(padint);
            }
            catch (Exception)
            {
                lock (objectsToMigrate)
                {
                    objectsToMigrate.Enqueue(padint);
                }
            }
        }


        public bool VerifyMigration(int uid)
        {
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            if ((uid % (nrServers + 1)) != id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Migrate(IPadInt padInt)
        {
            Console.WriteLine("Received migration of PadInt with id: " + ((PadInt) padInt).Id);
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");
                lock (this.specialObjects)
                {
                    this.specialObjects.Enqueue((PadInt) padInt);
                    Monitor.PulseAll(this.specialObjects);
                }
        }

        public IPadInt CreateReplicatedPadInt(int uid, TimeStamp timestamp)
        {
            PadInt padIntReplica = (PadInt)CreatePadInt(uid, timestamp);
       
            try
            {
                IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(uid);
                targetOfReplica.CreatePadInt(uid, timestamp);
            }
            catch (Exception e)
            {
                this.EnqueuePadInt(padIntReplica);
                Console.WriteLine("tenho cacada" + e.Message.ToString());
            }
            return padIntReplica;
        }

        public IPadInt CreatePadInt(int uid, TimeStamp timestamp)
        {
            PadInt padint = null;

            while (freezed)
                Thread.Sleep(1000);

            if (failed)
                throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            if (repository.ContainsKey(uid))
            {
                padint = repository[uid];
                if (!(padint.NextState == PadInt.NextStateEnum.TEMPORARY))
                    throw new TxCreateException("The uid " + uid + " already exists!");
                else
                    padint.Tries[timestamp] = new TryPadInt(timestamp, padint, padint.Value);
            }
            else
            {
                padint = new PadInt(uid, timestamp);
                this.repository[uid] = padint;
                try
                {
                    IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(uid);
                   // targetOfReplica.UpdatePadInt(new SerializablePadInt(padint.Value, padint.Id, padint.LastSuccessfulCommit, padint.LastSuccessfulCommit, padint.LastSuccessfulCommit, false));
                }
                catch (Exception)
                {
                    objectsToReplicate.Enqueue(padint);

                    lock (objectsToReplicate)
                    {
                        Monitor.PulseAll(objectsToReplicate);
                    }
                }
            }
            return padint;
        }
        public IPadInt ReplicatedAccessPadInt(int uid, TimeStamp timeStamp)
        {
            PadInt padIntReplica = (PadInt)AccessPadInt(uid, timeStamp);

            try
            {
                IServer targetOfReplica = ServerConnector.GetReplicationServerForObjectWithId(uid);
                targetOfReplica.AccessPadInt(uid, timeStamp);
            }
            catch (Exception e)
            {
                this.EnqueuePadInt(padIntReplica);
                Console.WriteLine("tenho cacadaReplicatedAccessPadInt" + e.Message.ToString());
            }
            return padIntReplica;
        }

        public IPadInt AccessPadInt(int uid, TimeStamp timeStamp)
        {
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            try
            {
                PadInt padint = this.repository[uid];

                //if (padint.NextState == PadInt.NextStateEnum.TEMPORARY)
                    //throw new TxAccessException("PadInt with uid " + uid + " doesn't exist!");
                padint.CreateTry(timeStamp);
                return padint;
            }
            catch (KeyNotFoundException)
            {
                if (this.receivedSpecialObjects.ContainsKey(uid))
                {
                    this.receivedSpecialObjects[uid].CreateTry(timeStamp);
                    return this.receivedSpecialObjects[uid];
                }
                else throw new TxAccessException("KeyNotFound");
            }
        }

        private static void CallVerifyChargeAsynchronous(ServerConnector.RemoteVerifyChargeAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            remoteFunction.EndInvoke(RemAr);
        }

        private static bool CallVerifyMigrationAsynchronous(int uid, ServerConnector.RemoteVerifyMigrationAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(uid, null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }

        private static void CallMigrateAsynchronous(int uid, ServerConnector.RemoteMigrateAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(uid, null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            remoteFunction.EndInvoke(RemAr);
        }

        private static IPadInt CallCreatePadIntAsynchronous(int uid, TimeStamp timeStamp, ServerConnector.RemoteCreatePadIntAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(uid, timeStamp, null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }

        private static IPadInt CallAccessPadIntAsynchronous(int uid, TimeStamp timeStamp, ServerConnector.RemoteAccessPadIntAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(uid, timeStamp, null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }

        public bool Freeze()
        {
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            return (freezed = true);
        }


        public bool Fail()
        {
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            return (failed = true);
        }


        public bool Status()
        {
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("-------------------------SERVER STATUS--------------------------");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("ID: " + Server.id);
            Console.WriteLine("URL: " + url + ":" + serverPort + endPoint);
            Console.WriteLine("Status: " + (freezed ? "Freezed" : (failed ? "Failed" : "Normal")));
            Console.WriteLine("Nr of stored PadInts: " + repository.Count);
            if (repository.Count > 0)
            {
                Console.Write("     Uid(s): { ");
                foreach (int key in repository.Keys)
                {
                    Console.Write(key + " | value: " + repository[key].Value + " ");
                }

                Console.WriteLine("}");
            }

            Console.WriteLine("Nr of received special PadInts: " + receivedSpecialObjects.Count);
            if (receivedSpecialObjects.Count > 0)
            {
                Console.Write("     Uid(s): { ");
                foreach (int key in receivedSpecialObjects.Keys)
                {
                    Console.Write(key + " | value: " + receivedSpecialObjects[key].Value + " ");
                }

                Console.WriteLine("}");
            }
            return true;
        }


        public bool Recover()
        {
            freezed = false;
            failed = false;
            return true;
        }

        public void EnqueuePadInt(PadInt padint)
        {
            objectsToReplicate.Enqueue(padint);

            lock (objectsToReplicate)
            {
                Monitor.PulseAll(objectsToReplicate);
            }
        }


        public void IncrementSystemCharge(int charge)
        {
            //Lock
            //Increment
            //Verify
            //Act
        }


        public bool HasPadIntWithId(int uid)
        {
            return this.repository.ContainsKey(uid) || this.receivedSpecialObjects.ContainsKey(uid);
        }
    }
}