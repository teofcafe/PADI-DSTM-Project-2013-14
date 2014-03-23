using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction;

namespace Library
{
    public interface IMaster
    {
        public Transaction.Transaction Connect();
        public Transaction.Transaction ConnectAgain(Transaction.Transaction transaction);
        public int RegisterServer(string ip);
        public void UnregisterServer(int id);
        public void NotifyOvercharge(int id);
        public string GetServerURL(int id);
        public int GetServerWithPadInt(int uid);
        public void NotifyNeedMigrate(int id, int uid);
    }
}
