using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using MasterLibrary;
using PADI_DSTM;
using DispersionLibrary;
using ServerLibrary;
using CoordinatorLibrary;


namespace Master
{
    public class Master : MarshalByRefObject, IMaster
    {
        private SimpleDispersionFormula dispersionFormula = new SimpleDispersionFormula();
        private Dictionary<int, string> availableServers; // <id, URL>
        private Dictionary<int, string> nonAvailableServers; // <id, URL>
        private Dictionary<int, int> specialPadInts; // <uid, id>
        private int maxTransactions;
        private int indexServers;
        private Random rand;

        public Master()
        {
            nonAvailableServers = new Dictionary<int, string>();
            availableServers = new Dictionary<int, string>();
            //possibleCoordinators = new ArrayList();
            specialPadInts = new Dictionary<int, int>();
            maxTransactions = 100; //VALOR PARA MUDAR
            indexServers = 0;
            rand = new Random();
        }

        public Transaction Connect()
        {
            //Generation of timestamp
            System.Console.WriteLine("Master.Connect() Called");

            TimeStamp timeStamp = new TimeStamp();

            //Creation of new transaction
            return new Transaction(timeStamp, this.GetServerURL() + "/Coordinator");
        }



        public IDispersionFormula GetDispersionFormula()
        {
            Console.WriteLine("Master.GetDispersionFormula() Called");

            return this.dispersionFormula;
        }

        public Transaction ConnectAgain(Transaction transaction)
        {
            throw new NotImplementedException();
        }


        public int RegisterServer(string url)
        {
            System.Console.WriteLine("Master.RegisterServer() Called with url: " + url);

            availableServers[indexServers] = url;

            this.dispersionFormula.NumberOfServers = this.indexServers + 1;

            return indexServers++;
        }
        public void UnregisterServer(int id)
        {
            try
            {
                availableServers.Remove(id);
            }
            catch (Exception)
            {
                nonAvailableServers.Remove(id);
            }
        }

        public void MarkAsOverCharged(int id)
        {
            try
            {
                nonAvailableServers.Add(id, availableServers[id]);
                availableServers.Remove(id);

            }
            catch (Exception)
            {

            }
        }
        public string GetServerURL(int id)
        {
            try
            {
                return availableServers[id].ToString();
            }
            catch (Exception)
            {
                return nonAvailableServers[id].ToString();
            }

        }

        public bool Status()
        {
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("-------------------------MASTER STATUS--------------------------");
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("Current TimeStamp: " + new TimeStamp().ToString());
            Console.WriteLine("Nr of Servers: " + (availableServers.Count + nonAvailableServers.Count));
            if ((availableServers.Count + nonAvailableServers.Count) > 0)
            {
                Console.Write("     Server(s): { ");
                foreach (KeyValuePair<int, string> server in availableServers)
                    Console.Write("(ID: " + server.Key + " -> URL: " + server.Value + ") ");
                foreach (KeyValuePair<int, string> server in nonAvailableServers)
                    Console.Write("(ID: " + server.Key + " -> URL: " + server.Value + ") ");
                Console.WriteLine("}");
            }

            for (int i = 0; i < availableServers.Count; i++)
            {
                CallStatusAsynchronous(new ServerConnector.RemoteAsyncDelegate(ServerConnector.GetServerWithURL((string)availableServers[i] + "/Server").Status));
                CallStatusAsynchronous(new CoordinatorConnector.RemoteAsyncDelegate(CoordinatorConnector.GetCoordinatorByUrl((string)availableServers[i] + "/Coordinator").Status));

            }

            for (int i = 0; i < nonAvailableServers.Count; i++)
            {
                CallStatusAsynchronous(new ServerConnector.RemoteAsyncDelegate(ServerConnector.GetServerWithURL((string)nonAvailableServers[i] + "/Server").Status));
                CallStatusAsynchronous(new CoordinatorConnector.RemoteAsyncDelegate(CoordinatorConnector.GetCoordinatorByUrl((string)nonAvailableServers[i] + "/Coordinator").Status));

            }


            return true;
        }

        private bool CallStatusAsynchronous(ServerConnector.RemoteAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }

        private bool CallStatusAsynchronous(CoordinatorConnector.RemoteAsyncDelegate remoteFunction)
        {
            IAsyncResult RemAr = remoteFunction.BeginInvoke(null, null);
            RemAr.AsyncWaitHandle.WaitOne();
            return remoteFunction.EndInvoke(RemAr);
        }


        public int GetServerWithPadInt(int uid)
        {
            throw new NotImplementedException();
        }
        public void NotifyNeedMigrate(int id, int uid)
        {
            throw new NotImplementedException();
        }

        public string GetServerURL()
        {
            string randomServer;
            int size = availableServers.Count;
            ArrayList lList = new ArrayList(availableServers.Keys);
            while (true)
            {
                int randomServerId = lList.IndexOf(rand.Next(lList.Count));
                randomServer = (string)availableServers[randomServerId];

                if (ServerConnector.GetServerWithURL(randomServer + "/" + ServerConnector.ServerEndPoint).VerifyOverCharge())
                    this.MarkAsOverCharged(randomServerId);
                else
                    break;
            }
            return randomServer;
        }

        public string GetServerURLToMigrate(int uid)
        {
            int randomServerID;

            if (availableServers.Count <= 2 &&
                    availableServers.Count > 0 &&
                    (availableServers.ElementAt(0).Key == this.dispersionFormula.GetIdOfServerWithObjectWithId(uid) ||
                    availableServers.ElementAt(0).Key == this.dispersionFormula.GetIdOfReplicaServerWithObjectWithId(uid)) &&
                    availableServers.Count > 1 &&
                    (availableServers.ElementAt(1).Key == this.dispersionFormula.GetIdOfServerWithObjectWithId(uid) ||
                    availableServers.ElementAt(1).Key == this.dispersionFormula.GetIdOfReplicaServerWithObjectWithId(uid)))
                    throw new TxNonAvailableServers("No existing servers to allow migration!!!");

                do
                {
                    randomServerID = this.availableServers.Keys.ElementAt(rand.Next(this.availableServers.Keys.Count));
                } while (availableServers.ElementAt(randomServerID).Key == this.dispersionFormula.GetIdOfServerWithObjectWithId(uid) ||
                    availableServers.ElementAt(randomServerID).Key == this.dispersionFormula.GetIdOfReplicaServerWithObjectWithId(uid));
                Console.WriteLine("Server escolhido para migrar: {0}", randomServerID);
                specialPadInts[uid] = randomServerID;
                return this.availableServers[randomServerID];
        }


        public int getServerIDForSpecialPadInt(int uid)
        {
            return specialPadInts[uid];
        }

        static void Main(string[] args)
        {

            TcpChannel channel = new TcpChannel(8089);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Master),
                "Master",
                WellKnownObjectMode.Singleton);

            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }
    }
}