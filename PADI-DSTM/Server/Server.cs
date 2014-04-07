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
using TransactionLibrary;
using System.Runtime.Serialization.Formatters;

namespace Server
{
    public class Server : MarshalByRefObject, IServer
    {

        private static int id, nrServers;

        //Carga maxima do server #Coordenar transaccao -> peso 1, #Read -> peso 2, #Write -> peso 3
        private int maxCharge, actualCharge;
        private bool overCharged;

        private Dictionary<int, IPadInt> repository = new Dictionary<int, IPadInt>();
        //hashtable[uid=4] = PadInt with value 4;

        //store the received special objects on this structure
        private Hashtable specialObjects = new Hashtable();

        // If the Master refuse the request of data migration, save the uids in this structure to migrate later. Ordered by nr of accesses.
        //Migrate only if 2 elements exists
        private int[] receivedSpecialObjects;

        private const string url = "tcp://localhost";

        private const string endPoint = "Server";

        private const string masterUrl = "tcp://localhost:8089/Master";

        public static void StartListening(string url, int port)
        {
            ChannelServices.RegisterChannel(new TcpChannel(port), true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Server),
                Server.endPoint,
                WellKnownObjectMode.Singleton);

            Coordinator.Coordinator.StartListening();

            //PadInt.dangerAcess = new PadInt.DangerAcess(extremAccessedObject);

            IMaster master = MasterConnector.GetMaster();
            Server.id = master.RegisterServer(url + ":" + port);
        }

        public Server()
        {
            //ThreadStart startDelegate = new ThreadStart(VerifyCharge);
            //Thread threadOne = new Thread(startDelegate);
            //threadOne.Priority = ThreadPriority.Lowest;
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
            while (true)
            {
                if (actualCharge > maxCharge)
                    overCharged = true;
                else
                    overCharged = false;
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        } 

        public bool VerifyMigration(int uid)
        {
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
            //itera sobre a lista, migra
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
        public void extremAccessedObject(PadInt padint) {
            if (this.receivedSpecialObjects.Length >= 1)
                this.Migrate(padint.Id);
        }


        public IPadInt CreatePadInt(int uid, TimeStamp timestamp)
        {
            PadInt padint = null;
            try
            {
                padint = new PadInt(uid);
                
                padint.NextState = PadInt.NextStateEnum.TEMPORARY;
                padint.LastSuccessfulCommit = timestamp;
                this.repository[uid] = padint;
            }
            catch (Exception e)
            {
                Console.WriteLine("Server. ## ## catchCreatePadInt: " + e.ToString());
            }

            return padint;
       }


        public IPadInt AccessPadInt(int uid, TimeStamp timeStamp)
        {
            return this.repository[uid];
        }
    }
}