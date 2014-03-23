using Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction;

namespace Master
{
    public class Master : IMaster
    {
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

        public Transaction.Transaction Connect()
        {
            //Generation of timestamp
            Int32 timeStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
            //Selection of random server
            int size = servers.Count;
            Random rand = new Random();
            string randomServer = (string)servers[rand.Next(size)];

            //Creation of new transaction
            return new Transaction.Transaction(timeStamp, randomServer);
        }
        public Transaction.Transaction ConnectAgain(Transaction.Transaction transaction)
        {
            throw new NotImplementedException();
        }
        public int RegisterServer(string ip)
        {
            servers[indexServers] = ip;

            return indexServers++;
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
    }
}
