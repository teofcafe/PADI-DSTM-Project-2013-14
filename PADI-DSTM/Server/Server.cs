using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coordinator;
using System.Threading;

namespace Server
{
    class Server : ICoordinator, IServer
    {

        private int id;

        //Carga maxima do server #Coordenar transaccao -> peso 1, #Read -> peso 2, #Write -> peso 3
        private int maxCharge, actualCharge;
        private bool overCharged;

        private Hashtable repository = new Hashtable();
        //hashtable[uid=4] = PadInt with value 4;

        //store the received special objects on this structure
        private Hashtable specialObjects = new Hashtable();

        // If the Master refuse the request of data migration, save the uids in this structure to migrate later. Ordered by nr of accesses.
        private int[] specialObjects;


        public Server(int id)
        {

            this.id = id;
            ThreadStart startDelegate = new ThreadStart(VerifyCharge);
            Thread threadOne = new Thread(startDelegate);
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

        int read(int uid)
        //adaptar metodo para PadInt, ver se o objecto que foi lido e especial, se for adiciona-lo ao vector para migrar. se
        //existirem dois objectos especiais, migrar um deles.
        {
            int padint = (int)repository[uid];
            padint++;
            return padint;
        }


        static void Main(string[] args)
        {
        }
    }
}