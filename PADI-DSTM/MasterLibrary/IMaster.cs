using DispersionLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

namespace MasterLibrary
{
    public interface IMaster
    {
        Transaction Connect();
        Transaction ConnectAgain(Transaction transaction);
        int RegisterServer(string ip);
        void UnregisterServer(int id);
        void NotifyOvercharge(int id);
        string GetServerURL(int id);
        int GetServerWithPadInt(int uid);
        void NotifyNeedMigrate(int id, int uid);
        IDispersionFormula GetDispersionFormula();
        bool Status();
    }
}
