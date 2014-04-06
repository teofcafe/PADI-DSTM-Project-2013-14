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

        private Hashtable repository = new Hashtable();
        private Hashtable temporaryPadInts = new Hashtable();
        //hashtable[uid=4] = PadInt with value 4;

        //store the received special objects on this structure
        private Hashtable specialObjects = new Hashtable();

        // If the Master refuse the request of data migration, save the uids in this structure to migrate later. Ordered by nr of accesses.
        //Migrate only if 2 elements exists
        private int[] receivedSpecialObjects;

        private TcpChannel channel = null;

        private const int port = 8082;

        private const string url = "tcp://localhost";

        private const string endPoint = "Server";

        private const string masterUrl = "tcp://localhost:8089/Master";

        private static string serverUrl = url + ":" + port;

        public static void StartListening(string url, int port)
        {
            Console.WriteLine("Server.StartListening() Called on " + Server.serverUrl);

            ChannelServices.RegisterChannel(new TcpChannel(port), true);

            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Server),
                Server.endPoint,
                WellKnownObjectMode.Singleton);

            Coordinator.Coordinator.StartListening();

            //PadInt.dangerAcess = new PadInt.DangerAcess(extremAccessedObject);

            IMaster master = (IMaster)Activator.GetObject(typeof(IMaster), masterUrl);
            Server.id = master.RegisterServer(Server.serverUrl);

            Console.WriteLine("Server.Server(): My ID is " + Server.id);
        }

        public Server()
        {
            Console.WriteLine("Server.Server() Called ");
            //ThreadStart startDelegate = new ThreadStart(VerifyCharge);
            //Thread threadOne = new Thread(startDelegate);
            //threadOne.Priority = ThreadPriority.Lowest;
        }
   
        static void Main(string[] args)
        {
            Server.StartListening("tcp://localhost:8082", 8082);
            
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
                Console.WriteLine("Server.CreatePadInt: Criei PadInt com uid = " + uid);
                Console.WriteLine("Server. ## ## Entrarrrrr ja criei a inst de padInt: ");
                temporaryPadInts[timestamp] = uid;
                //if (this.VerifyMigration(uid))
                    //padint.NextState = PadInt.NextStateEnum.MIGRATE;
                Console.WriteLine("Server.CreatePadInt: Vou enviar o PadInt com uid = " + uid);
            }
            catch (Exception e)
            {
                Console.WriteLine("Server. ## ## catchCreatePadInt: " + e.ToString());
            }

            return padint;
       }


        public IPadInt CreatePadInt(int uid)
        {
            throw new NotImplementedException();
        }
    }
}