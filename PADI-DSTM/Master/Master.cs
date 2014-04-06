﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using MasterLibrary;
using TransactionLibrary;
using DispersionLibrary;

namespace Master
{
    public class Master : MarshalByRefObject, IMaster
    {
        private SimpleDispersionFormula dispersionFormula = new SimpleDispersionFormula();
        private Hashtable servers; // <id, URL>
        //private ArrayList possibleCoordinators; // URL
        private Hashtable specialPadInts; // <uid, id>
        private int maxTransactions;
        private int indexServers;

        public Master()
        {
            servers = new Hashtable();
            //possibleCoordinators = new ArrayList();
            specialPadInts = new Hashtable();
            maxTransactions = 100; //VALOR PARA MUDAR
            indexServers = 0;
        }

        public Transaction Connect()
        {
            //Generation of timestamp
            System.Console.WriteLine("Master.Connect() Called");
            
            TimeStamp timeStamp = new TimeStamp();
            //Selection of random server
            int size = servers.Count;
            Random rand = new Random();
            string randomServer = (string)servers[rand.Next(size)];

            //Creation of new transaction
            return new Transaction(timeStamp, randomServer + "/Coordinator");
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

            servers[indexServers] = url;
            this.dispersionFormula.NumberOfServers = ++this.indexServers;

            return indexServers;
        }
        public void UnregisterServer(int id)
        {
            servers.Remove(id);
        }
        public void NotifyOvercharge(int id)
        {
            throw new NotImplementedException();
        }
        public string GetServerURL(int id)
        {
            return servers[id].ToString();
        }
        public int GetServerWithPadInt(int uid)
        {
            throw new NotImplementedException();
        }
        public void NotifyNeedMigrate(int id, int uid)
        {
            throw new NotImplementedException();
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