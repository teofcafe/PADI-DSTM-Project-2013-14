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
using Library;
using System.Net.Sockets;

namespace Server
{
    class Server : ICoordinator, IServer, MarshalByRefObject
    {

        private int id, nrServers;
        String url;

        //Carga maxima do server #Coordenar transaccao -> peso 1, #Read -> peso 2, #Write -> peso 3
        private int maxCharge, actualCharge;
        private bool overCharged;

        private Hashtable repository = new Hashtable();
        //hashtable[uid=4] = PadInt with value 4;

        //store the received special objects on this structure
        private Hashtable specialObjects = new Hashtable();

        // If the Master refuse the request of data migration, save the uids in this structure to migrate later. Ordered by nr of accesses.
        private int[] specialObjects;

        private TcpChannel channel = null;


        public Server()
        {

            ThreadStart startDelegate = new ThreadStart(VerifyCharge);
            Thread threadOne = new Thread(startDelegate);
            threadOne.Priority = ThreadPriority.Lowest;
            url = "tcp: //localhost:8082 /Server";
           

            try
            {
                channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, true);
                IMaster master = (IMaster)Activator.GetObject(typeof(IServer),"tcp://localhost:8089/Master");
                this.id = master.RegisterServer(url);
                Console.WriteLine("O meu ID e " + this.id);
            }
            catch (SocketException exc)
            {
               Console.WriteLine("nao liguei");
            }
        }

        void VerifyCharge()
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

        void VerifyMigration(int uid) {
            if ((uid % (nrServers + 1)) != id)
            {
                //Mark PadInt object to migrate
            }
            else
            {
                //Mark PadInt object to none
            }
        }

        void Migrate(int[] servers)
        {
            //itera sobre a lista, migra
        }

       /* PadInt CreatePadInt(int uid)
        {
            PadInt padint = new PadInt(uid);
            verifyMigration(uid);
            repository[uid] = padint;
            return padint;

        }*/

        int read(int uid)
        //adaptar metodo para PadInt, ver se o objecto que foi lido e especial, se for adiciona-lo ao vector para migrar. se
        //existirem dois objectos especiais, migrar um deles.
        {
            //lerPadint
            //IncrementarAcesso
            //verificar se e especial
            //se for marcar pa migrar
            //retornar PAdInt
            return 0;
        }


        static void Main(string[] args)
        {
            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }
    }
}