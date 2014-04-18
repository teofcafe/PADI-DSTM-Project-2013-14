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
        private Hashtable specialObjects = new Hashtable();

        // If the Master refuse the request of data migration, save the uids in this structure to migrate later. Ordered by nr of accesses.
        //Migrate only if 2 elements exists
        private int[] receivedSpecialObjects;

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
        }

        static void Main(string[] args)
        {
            int port;

            Console.Write("Port: ");
            if (!int.TryParse(Console.ReadLine(), out port))
            {
                Console.WriteLine("Invalid Port!!!!!");
                port = 8082;
            }


            Server.StartListening("tcp://localhost", port);

            Console.WriteLine("Listening on URL: {0}:{1}", "tcp://localhost", port);
            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }

        public void VerifyCharge()
        {
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            while (true)
            {
                if (actualCharge > maxCharge)
                    overCharged = true;
                else
                    overCharged = false;
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
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
            this.repository.Remove(padint.Id);
            //TODO
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

        public void Migrate(int uid)
        {
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            throw new NotImplementedException();
        }

        public int Read(int uid, TimeStamp timestamp)
        {
            PadInt padint = (PadInt)repository[uid];
            actualCharge = actualCharge + 2;
            return padint.Read(timestamp);
        }

        public void Write(int uid, TimeStamp timestamp, int value)
        {
            PadInt padint = (PadInt)repository[uid];
            actualCharge = actualCharge + 3;
            padint.Write(value, timestamp);
        }

        //Esta a migrar o ultimo recebido. Depois implementar: migrar o que tem mais acessos;
        public void extremAccessedObject(PadInt padint)
        {
            if (this.receivedSpecialObjects.Length >= 1)
                this.Migrate(padint.Id);
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
                    padint.Tries[timestamp] = padint.Value;
            }
            else
            {
                padint = new PadInt(uid, timestamp);
                this.repository[uid] = padint;
            }
            return padint;
        }


        public IPadInt AccessPadInt(int uid, TimeStamp timeStamp)
        {
            while (freezed)
                Thread.Sleep(1000);

            if (failed) throw new TxFailedException("The server " + url + ":" + serverPort + " is down!");

            try
            {
                PadInt padint = this.repository[uid];

                if (padint.NextState == PadInt.NextStateEnum.TEMPORARY) 
                    throw new TxAccessException("PadInt with uid " + uid + " doesn't exist!");
                padint.Tries[timeStamp] = padint.Value;
                return padint;
            }
            catch (KeyNotFoundException)
            {
                throw new TxAccessException("PadInt with uid " + uid + " doesn't exist!");
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
                    Console.Write(key + " ");
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
    }
}